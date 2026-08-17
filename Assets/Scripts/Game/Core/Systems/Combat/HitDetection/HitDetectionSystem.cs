using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.Data;
using Game.Core.Entities;
using Game.Core.Extensions;
using Game.Core.GameEvents;
using Game.Core.Level;
using Game.Core.Level.Entities;
using UnityEngine;
using Zenject;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Systems
{
    public sealed class HitDetectionSystem : IHitDetectionSystem, IFixedTickable
    {
        private const int HitBufferCapacity = 32;
        private const float MinimumDirectionSqrMagnitude = 0.0001f;

        private readonly IGameEventsBus _events;
        private readonly ICharacterContext _characters;
        private readonly ICharacterViewContext _characterViews;
        private readonly ILevelEntityRegistry _levelEntities;
        private readonly Collider[] _hitBuffer = new Collider[HitBufferCapacity];
        private readonly Dictionary<EntityId, HitDetectionState> _states = new(16);
        private readonly int _targetLayerMask =
            (1 << LayerData.Hitbox) | (1 << LayerData.Environment);

        public HitDetectionSystem(
            IGameEventsBus events,
            ICharacterContext characters,
            ICharacterViewContext characterViews,
            ILevelEntityRegistry levelEntities
        )
        {
            _events = events;
            _characters = characters;
            _characterViews = characterViews;
            _levelEntities = levelEntities;

            _events.Subscribe<OnSimpleAttackStartedEvent>(OnAttackStarted);
            _events.Subscribe<OnSimpleAttackEndedEvent>(OnAttackEnded);
            _events.Subscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);
            _events.Subscribe<OnPowerAttackEndedEvent>(OnPowerAttackEnded);
            _characters.OnCharacterAdded += Register;
            _characters.OnCharacterRemoved += Unregister;

            IReadOnlyList<ICharacterModel> existingCharacters = _characters.AllCharacters;
            for (int i = 0; i < existingCharacters.Count; i++)
            {
                Register(existingCharacters[i].CharacterID);
            }
        }

        public void FixedTick()
        {
            foreach (KeyValuePair<EntityId, HitDetectionState> pair in _states)
            {
                HitDetectionState state = pair.Value;
                if (!state.IsActive)
                {
                    continue;
                }

                ICharacterModel attacker = _characters.GetModel(pair.Key);
                if (attacker == null)
                {
                    state.Reset();
                    continue;
                }

                ICharacterCombatRuntimeState combat =
                    attacker.GetState<ICharacterCombatRuntimeState>();
                if (!combat.Enabled)
                {
                    state.Reset();
                    continue;
                }

                state.Elapsed += Time.fixedDeltaTime;
                if (state.AttackType == AttackType.Simple)
                {
                    DetectSimpleAttackHits(combat, state);
                }
                else
                {
                    DetectPowerAttackHits(combat, state);
                }
            }
        }

        private void DetectSimpleAttackHits(
            ICharacterCombatRuntimeState attacker,
            HitDetectionState state
        )
        {
            if (state.Elapsed < state.ActiveWindowStart ||
                state.Elapsed >= state.ActiveWindowEnd)
            {
                return;
            }

            Vector3 forward = GetHorizontalDirection(attacker.Forward);
            if (forward == Vector3.zero)
            {
                return;
            }

            Vector3 start = attacker.AttackOrigin;
            Vector3 end = start + forward * state.HitboxRange;
            int hitCount = CollisionsExtension.OverlapCapsule(
                start,
                end,
                state.HitboxRadius,
                _hitBuffer,
                _targetLayerMask);
            PublishOverlapHits(attacker, state, hitCount, AttackType.Simple);
        }

        private void DetectPowerAttackHits(
            ICharacterCombatRuntimeState attacker,
            HitDetectionState state
        )
        {
            if (state.Elapsed < state.ActiveWindowStart)
            {
                return;
            }

            PublishPowerAttackCharacterHits(attacker, state);
            int hitCount = CollisionsExtension.OverlapSphere(
                attacker.Position,
                state.HitboxRadius,
                _hitBuffer,
                1 << LayerData.Environment);
            PublishOverlapHits(attacker, state, hitCount, AttackType.Power);
            state.Reset();
        }

        private void PublishOverlapHits(
            ICharacterCombatRuntimeState attacker,
            HitDetectionState state,
            int hitCount,
            AttackType attackType
        )
        {
            for (int i = 0; i < hitCount; i++)
            {
                Collider hitbox = _hitBuffer[i];
                _hitBuffer[i] = null;
                if (hitbox == null)
                {
                    continue;
                }

                if (_characterViews.TryGetCharacterId(hitbox, out EntityId characterId))
                {
                    ICharacterModel target = _characters.GetModel(characterId);
                    if (target == null)
                    {
                        continue;
                    }

                    ICharacterCombatRuntimeState targetCombat =
                        target.GetState<ICharacterCombatRuntimeState>();
                    if (targetCombat.Enabled &&
                        targetCombat.CharacterID != attacker.CharacterID &&
                        state.HitTargets.Add(targetCombat.CharacterID))
                    {
                        PublishCharacterHit(
                            attacker,
                            targetCombat,
                            attackType);
                    }

                    continue;
                }

                if (!_levelEntities.TryGetEntityId(hitbox, out EntityId entityId) ||
                    !_levelEntities.TryGetEntity(
                        entityId,
                        out ILevelEntityView entity))
                {
                    continue;
                }

                Vector3 direction = entity.Position - attacker.Position;
                if (attackType == AttackType.Power &&
                    Mathf.Abs(direction.y) > state.HitboxHeight)
                {
                    continue;
                }

                if (!state.HitTargets.Add(entityId))
                {
                    continue;
                }

                PublishLevelEntityHit(attacker, entity, attackType);
            }
        }

        private void PublishPowerAttackCharacterHits(
            ICharacterCombatRuntimeState attacker,
            HitDetectionState state
        )
        {
            float radiusSqr = state.HitboxRadius * state.HitboxRadius;
            IReadOnlyList<ICharacterModel> characters = _characters.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                ICharacterCombatRuntimeState target =
                    characters[i].GetState<ICharacterCombatRuntimeState>();
                Vector3 direction = target.Position - attacker.Position;
                if (Mathf.Abs(direction.y) > state.HitboxHeight)
                {
                    continue;
                }

                direction.y = 0f;
                if (direction.sqrMagnitude <= radiusSqr &&
                    target.Enabled &&
                    target.CharacterID != attacker.CharacterID &&
                    state.HitTargets.Add(target.CharacterID))
                {
                    PublishCharacterHit(attacker, target, AttackType.Power);
                }
            }
        }

        private void PublishCharacterHit(
            ICharacterCombatRuntimeState attacker,
            ICharacterCombatRuntimeState target,
            AttackType attackType
        )
        {
            Vector3 direction = GetHorizontalDirection(target.Position - attacker.Position);
            if (direction == Vector3.zero)
            {
                direction = GetHorizontalDirection(attacker.Forward);
            }

            _events.Publish(new OnHitDetectedEvent(new HitData(
                HitObjectType.Character,
                attacker.CharacterID,
                HitObjectType.Character,
                target.CharacterID,
                attackType,
                direction)));
        }

        private void PublishLevelEntityHit(
            ICharacterCombatRuntimeState attacker,
            ILevelEntityView entity,
            AttackType attackType
        )
        {
            Vector3 direction = GetHorizontalDirection(entity.Position - attacker.Position);
            if (direction == Vector3.zero)
            {
                direction = GetHorizontalDirection(attacker.Forward);
            }

            _events.Publish(new OnHitDetectedEvent(new HitData(
                HitObjectType.Character,
                attacker.CharacterID,
                HitObjectType.LevelEntity,
                entity.EntityId,
                attackType,
                direction)));
        }

        private void OnAttackStarted(OnSimpleAttackStartedEvent evt)
        {
            ICharacterModel attacker = _characters.GetModel(evt.CharacterID);
            if (attacker == null || !_states.TryGetValue(evt.CharacterID, out HitDetectionState state))
            {
                return;
            }

            SimpleAttackSettings settings = attacker.Data.Combat.SimpleAttack;
            state.BeginAttack(
                settings.Duration * settings.HitboxStartNormalized,
                settings.Duration * settings.HitboxEndNormalized,
                settings.HitboxRange,
                settings.HitboxRadius);
        }

        private void OnAttackEnded(OnSimpleAttackEndedEvent evt)
        {
            if (_states.TryGetValue(evt.CharacterID, out HitDetectionState state) &&
                state.AttackType == AttackType.Simple)
            {
                state.Reset();
            }
        }

        private void OnPowerAttackStarted(OnPowerAttackStartedEvent evt)
        {
            ICharacterModel attacker = _characters.GetModel(evt.CharacterID);
            if (attacker == null || !_states.TryGetValue(evt.CharacterID, out HitDetectionState state))
            {
                return;
            }

            PowerAttackSettings settings = attacker.Data.Combat.PowerAttack;
            state.BeginPowerAttack(
                settings.WaveStartTime,
                settings.WaveRadius,
                settings.WaveHeight);
        }

        private void OnPowerAttackEnded(OnPowerAttackEndedEvent evt)
        {
            if (_states.TryGetValue(evt.CharacterID, out HitDetectionState state) &&
                state.AttackType == AttackType.Power)
            {
                state.Reset();
            }
        }

        private void Register(EntityId characterId)
        {
            if (!_states.ContainsKey(characterId))
            {
                _states.Add(characterId, new HitDetectionState());
            }
        }

        private void Unregister(EntityId characterId)
            => _states.Remove(characterId);

        private static Vector3 GetHorizontalDirection(Vector3 direction)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude < MinimumDirectionSqrMagnitude)
            {
                return Vector3.zero;
            }

            direction.Normalize();
            return direction;
        }

        public void Dispose()
        {
            _events.Unsubscribe<OnSimpleAttackStartedEvent>(OnAttackStarted);
            _events.Unsubscribe<OnSimpleAttackEndedEvent>(OnAttackEnded);
            _events.Unsubscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);
            _events.Unsubscribe<OnPowerAttackEndedEvent>(OnPowerAttackEnded);
            _characters.OnCharacterAdded -= Register;
            _characters.OnCharacterRemoved -= Unregister;
        }
    }
}

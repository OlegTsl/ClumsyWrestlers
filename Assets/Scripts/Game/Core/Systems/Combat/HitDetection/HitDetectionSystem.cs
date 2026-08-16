using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.Data;
using Game.Core.Extensions;
using Game.Core.GameEvents;
using Game.Core.Environment;
using Game.Core.Level;
using UnityEngine;
using Zenject;

namespace Game.Core.Systems
{
    public sealed class HitDetectionSystem : IHitDetectionSystem, IFixedTickable
    {
        private const int   CHitBufferCapacity            = 32;
        private const float CMinimumImpactSpeed           = 0.3f;
        private const float CMinimumDirectionSqrMagnitude = 0.0001f;

        private readonly GameEventsBus     _gameEventsBus;
        private readonly ICharacterContext _context;
        private readonly ILevelModel       _level;
        private readonly Collider[] _hitBuffer = new Collider[CHitBufferCapacity];
        private readonly Dictionary<Guid, HitDetectionState> _states = new();
        private readonly int _targetLayerMask =
            (1 << LayerData.Hitbox) | (1 << LayerData.Environment);

        public HitDetectionSystem(
            GameEventsBus     gameEventsBus,
            ICharacterContext context,
            ILevelModel       level
        )
        {
            _gameEventsBus = gameEventsBus;
            _context       = context;
            _level         = level;

            _gameEventsBus.Subscribe<OnSimpleAttackStartedEvent>(OnAttackStarted);
            _gameEventsBus.Subscribe<OnSimpleAttackEndedEvent>(OnAttackEnded);
            _gameEventsBus.Subscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);
            _gameEventsBus.Subscribe<OnPowerAttackEndedEvent>(OnPowerAttackEnded);

            _context.OnCharacterAdded   += Register;
            _context.OnCharacterRemoved += Unregister;

            RegisterExistingCharacters();
        }

        public void FixedTick()
        {
            foreach (var pair in _states)
            {
                var state = pair.Value;
                if (!state.IsActive)
                    continue;

                var attacker = _context.GetModel(pair.Key);
                if (attacker == null || !attacker.Enabled)
                {
                    state.Reset();
                    continue;
                }

                state.Elapsed += Time.fixedDeltaTime;
                
                if (state.AttackType == AttackType.Simple)
                    DetectSimpleAttackHits(attacker, state);
                else
                    DetectPowerAttackHits(attacker, state);
            }

            DetectEnvironmentCollisions();
        }

        private void DetectSimpleAttackHits(ICharacterModel attacker, HitDetectionState state)
        {
            if (state.Elapsed < state.ActiveWindowStart ||
                state.Elapsed >= state.ActiveWindowEnd)
            {
                return;
            }

            Vector3 forward = attacker.Forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < CMinimumDirectionSqrMagnitude)
                return;

            forward.Normalize();

            Vector3 start = attacker.AttackOrigin;
            Vector3 end   = start + forward * state.HitboxRange;

            int hits = CollisionsExtension.OverlapCapsule(start, end,
                state.HitboxRadius, _hitBuffer, _targetLayerMask);

            PublishSimpleAttackHits(attacker, state, hits);
        }

        private void PublishSimpleAttackHits(
            ICharacterModel attacker,
            HitDetectionState state,
            int hits
        )
        {
            for (int i = 0; i < hits; i++)
            {
                Collider hitCollider = _hitBuffer[i];
                _hitBuffer[i] = null;

                if (hitCollider == null)
                    continue;

                ICharacterModel target = _context.GetModel(hitCollider);
                if (target != null)
                {
                    if (!target.Enabled || target.CharacterID == attacker.CharacterID ||
                        !state.HitTargets.Add(target.CharacterID))
                    {
                        continue;
                    }

                    PublishCharacterHit(attacker, target, AttackType.Simple);
                    continue;
                }

                IInteractableChestView chest = _level.GetInteractableChest(hitCollider);
                
                if (!state.HitTargets.Add(chest.ChestID))
                    continue;

                PublishChestHit(attacker, chest, AttackType.Simple);
            }
        }

        private void DetectPowerAttackHits(ICharacterModel attacker, HitDetectionState state)
        {
            if (state.Elapsed < state.ActiveWindowStart)
                return;

            PublishPowerAttackCharacterHits(attacker, state);

            int hits = CollisionsExtension.OverlapSphere(attacker.Position,
                state.HitboxRadius, _hitBuffer, 1 << LayerData.Environment);
            
            for (int i = 0; i < hits; i++)
            {
                Collider hitCollider = _hitBuffer[i];
                _hitBuffer[i] = null;

                IInteractableChestView chest = _level.GetInteractableChest(hitCollider);
                Vector3 direction = chest.Position - attacker.Position;
                
                if (Mathf.Abs(direction.y) <= state.HitboxHeight)
                    PublishChestHit(attacker, chest, AttackType.Power);
            }

            state.Reset();
        }

        private void PublishPowerAttackCharacterHits(ICharacterModel attacker, HitDetectionState state)
        {
            float radiusSqr = state.HitboxRadius * state.HitboxRadius;
            IReadOnlyList<ICharacterModel> characters = _context.AllCharacters;
            
            for (int i = 0; i < characters.Count; i++)
            {
                ICharacterModel target = characters[i];
                Vector3 direction = target.Position - attacker.Position;

                if (Mathf.Abs(direction.y) > state.HitboxHeight)
                    continue;

                direction.y = 0f;

                if (direction.sqrMagnitude > radiusSqr)
                    continue;

                PublishCharacterHit(attacker, target, AttackType.Power);
            }
        }

        private void PublishCharacterHit(ICharacterModel attacker, ICharacterModel target, AttackType attackType)
        {
            if (!target.Enabled || target.CharacterID == attacker.CharacterID)
                return;

            Vector3 hitDirection = GetHorizontalDirection(target.Position - attacker.Position);
            
            if (hitDirection.sqrMagnitude < CMinimumDirectionSqrMagnitude)
                hitDirection = GetHorizontalDirection(attacker.Forward);

            _gameEventsBus.Publish(new OnHitDetectedEvent(new HitData(
                HitObjectType.Character, attacker.CharacterID,
                HitObjectType.Character, target.CharacterID,
                attackType, hitDirection)));
        }

        private void PublishChestHit(ICharacterModel attacker, IInteractableChestView chest, AttackType attackType)
        {
            Vector3 hitDirection = GetHorizontalDirection(chest.Position - attacker.Position);
            
            if (hitDirection.sqrMagnitude < CMinimumDirectionSqrMagnitude)
                hitDirection = GetHorizontalDirection(attacker.Forward);

            _gameEventsBus.Publish(new OnHitDetectedEvent(new HitData(
                HitObjectType.Character, attacker.CharacterID,
                HitObjectType.Environment, chest.ChestID,
                attackType, hitDirection)));
        }

        private void DetectEnvironmentCollisions()
        {
            IReadOnlyList<IInteractableChestView> chests = _level.InteractableChests;
            for (int i = 0; i < chests.Count; i++)
            {
                IInteractableChestView chest = chests[i];
                
                if (!chest.TryConsumeCollision(out EnvironmentCollisionData collision) ||
                    collision.ImpactSpeed < CMinimumImpactSpeed)
                {
                    continue;
                }

                ICharacterModel target = GetCollisionTarget(collision.OtherCollider);
                if (target == null || !target.Enabled)
                    continue;

                Vector3 hitDirection = target.Position - chest.Position;
                
                _gameEventsBus.Publish(new OnHitDetectedEvent(new HitData(
                    HitObjectType.Environment, chest.ChestID,
                    HitObjectType.Character, target.CharacterID,
                    AttackType.Simple, hitDirection, collision.ImpactVelocity)));
            }
        }

        private ICharacterModel GetCollisionTarget(Collider collider)
        {
            ICharacterModel target = _context.GetModel(collider);
            if (target != null)
                return target;

            Rigidbody targetRigidbody = collider.attachedRigidbody;
            if (targetRigidbody == null)
                return null;

            IReadOnlyList<ICharacterModel> characters = _context.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                ICharacterModel character = characters[i];
                
                if (character.Hitbox.attachedRigidbody == targetRigidbody)
                    return character;
            }

            return null;
        }

        private void OnAttackStarted(OnSimpleAttackStartedEvent evt)
        {
            if (!_states.TryGetValue(evt.CharacterID, out var state))
                return;

            ICharacterModel      attacker = _context.GetModel(evt.CharacterID);
            SimpleAttackSettings settings = attacker.Data.Combat.SimpleAttack;

            state.BeginAttack(
                settings.Duration * settings.HitboxStartNormalized,
                settings.Duration * settings.HitboxEndNormalized,
                settings.HitboxRange, settings.HitboxRadius);
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
            if (!_states.TryGetValue(evt.CharacterID, out HitDetectionState state))
                return;

            PowerAttackSettings settings = _context.GetModel(evt.CharacterID).Data.Combat.PowerAttack;
            state.BeginPowerAttack(settings.WaveStartTime, settings.WaveRadius, settings.WaveHeight);
        }

        private void OnPowerAttackEnded(OnPowerAttackEndedEvent evt)
        {
            if (_states.TryGetValue(evt.CharacterID, out HitDetectionState state) &&
                state.AttackType == AttackType.Power)
            {
                state.Reset();
            }
        }

        private void RegisterExistingCharacters()
        {
            var characters = _context.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                Register(characters[i].CharacterID);
            }
        }

        private void Register(Guid characterID)
        {
            if (!_states.ContainsKey(characterID))
                _states[characterID] = new HitDetectionState();
        }

        private void Unregister(Guid characterID)
            => _states.Remove(characterID);

        private static Vector3 GetHorizontalDirection(Vector3 direction)
        {
            direction.y = 0f;
            
            if (direction.sqrMagnitude >= CMinimumDirectionSqrMagnitude)
                direction.Normalize();

            return direction;
        }

        public void Dispose()
        {
            _gameEventsBus.Unsubscribe<OnSimpleAttackStartedEvent>(OnAttackStarted);
            _gameEventsBus.Unsubscribe<OnSimpleAttackEndedEvent>(OnAttackEnded);
            _gameEventsBus.Unsubscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);
            _gameEventsBus.Unsubscribe<OnPowerAttackEndedEvent>(OnPowerAttackEnded);

            _context.OnCharacterAdded   -= Register;
            _context.OnCharacterRemoved -= Unregister;
        }
    }
}

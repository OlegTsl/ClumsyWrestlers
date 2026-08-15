using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.Data;
using Game.Core.Extensions;
using Game.Core.GameEvents;
using UnityEngine;
using Zenject;

namespace Game.Core.Systems
{
    public sealed class HitDetectionSystem : IHitDetectionSystem, IFixedTickable
    {
        private const int   CHitBufferCapacity            = 32;
        private const float CMinimumDirectionSqrMagnitude = 0.0001f;

        private readonly GameEventsBus     _gameEventsBus;
        private readonly ICharacterContext _context;
        private readonly Collider[] _hitBuffer = new Collider[CHitBufferCapacity];
        private readonly Dictionary<Guid, HitDetectionState> _states = new();
        private readonly int _hitboxLayerMask = 1 << LayerData.Hitbox;

        public HitDetectionSystem(
            GameEventsBus     gameEventsBus,
            ICharacterContext context
        )
        {
            _gameEventsBus = gameEventsBus;
            _context       = context;

            _gameEventsBus.Subscribe<OnSimpleAttackStartedEvent>(OnAttackStarted);
            _gameEventsBus.Subscribe<OnSimpleAttackEndedEvent>(OnAttackEnded);

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
                DetectHits(attacker, state);
            }
        }

        private void DetectHits(ICharacterModel attacker, HitDetectionState state)
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
                state.HitboxRadius, _hitBuffer, _hitboxLayerMask);

            PublishDetectedHits(attacker, hits);
        }

        private void PublishDetectedHits(ICharacterModel attacker, int hits)
        {
            for (int i = 0; i < hits; i++)
            {
                Collider hitCollider = _hitBuffer[i];
                _hitBuffer[i] = null;

                if (hitCollider == null)
                    continue;

                var target = _context.GetModel(hitCollider);
                if (target == null || !target.Enabled ||
                    target.CharacterID == attacker.CharacterID)
                {
                    continue;
                }

                Vector3 hitDirection = target.Position - attacker.Position;
                hitDirection.y = 0f;

                if (hitDirection.sqrMagnitude < CMinimumDirectionSqrMagnitude)
                    hitDirection = attacker.Forward;

                hitDirection.Normalize();

                _gameEventsBus.Publish(new OnSimpleAttackHitDetectedEvent(
                    attacker.CharacterID, target.CharacterID, hitDirection));
            }
        }

        private void OnAttackStarted(OnSimpleAttackStartedEvent evt)
        {
            if (!_states.TryGetValue(evt.CharacterID, out var state))
                return;

            var attacker = _context.GetModel(evt.CharacterID);
            SimpleAttackSettings settings = attacker?.Data.Combat.SimpleAttack;
            
            if (settings == null || settings.Duration <= 0f)
            {
                state.Reset();
                return;
            }

            state.BeginAttack(
                settings.Duration * settings.HitboxStartNormalized,
                settings.Duration * settings.HitboxEndNormalized,
                settings.HitboxRange, settings.HitboxRadius);
        }

        private void OnAttackEnded(OnSimpleAttackEndedEvent evt)
        {
            if (!_states.TryGetValue(evt.CharacterID, out var state))
                return;

            state.Reset();
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

        public void Dispose()
        {
            _gameEventsBus.Unsubscribe<OnSimpleAttackStartedEvent>(OnAttackStarted);
            _gameEventsBus.Unsubscribe<OnSimpleAttackEndedEvent>(OnAttackEnded);

            _context.OnCharacterAdded   -= Register;
            _context.OnCharacterRemoved -= Unregister;
        }
    }
}

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
    public sealed class SimpleAttackHitDetectionSystem :
        ISimpleAttackHitDetectionSystem,
        IFixedTickable
    {
        private const int   HitBufferCapacity = 64;
        private const float MinimumDirectionSqrMagnitude = 0.0001f;

        private readonly GameEventsBus     _gameEventsBus;
        private readonly ICharacterContext _context;
        private readonly Collider[] _hitBuffer = new Collider[HitBufferCapacity];
        private readonly Dictionary<Guid, SimpleAttackHitDetectionState> _states = new();
        private readonly int _hitboxLayerMask = 1 << LayerData.Hitbox;

        public SimpleAttackHitDetectionSystem(
            GameEventsBus     gameEventsBus,
            ICharacterContext context
        )
        {
            _gameEventsBus = gameEventsBus;
            _context       = context;

            _gameEventsBus.Subscribe<OnAttackStartedEvent>(OnAttackStarted);
            _gameEventsBus.Subscribe<OnAttackEndedEvent>(OnAttackEnded);

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
                DetectHitsDuringActiveWindow(attacker, state.Elapsed);
            }
        }

        private void DetectHitsDuringActiveWindow(ICharacterModel attacker, float elapsed)
        {
            var settings = attacker.Data.Combat;
            if (elapsed < settings.SimpleAttackHitboxStart ||
                elapsed >= settings.SimpleAttackHitboxEnd)
            {
                return;
            }

            Vector3 forward = attacker.Forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < MinimumDirectionSqrMagnitude)
                return;

            forward.Normalize();

            Vector3 center = attacker.Position + Vector3.up * settings.SimpleAttackHitboxHeight;
            Vector3 start  = center  + forward * settings.SimpleAttackHitboxForwardOffset;
            Vector3 end    = start   + forward * settings.SimpleAttackHitboxRange;

            int hitCount = CollisionsExtension.OverlapCapsule(start, end,
                settings.SimpleAttackHitboxRadius, _hitBuffer, _hitboxLayerMask);

            PublishDetectedHits(attacker, settings.SimpleAttackForce, hitCount);
        }

        private void PublishDetectedHits(ICharacterModel attacker, float force, int hitCount)
        {
            for (int i = 0; i < hitCount; i++)
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

                _gameEventsBus.Publish(new OnDamageEvent(
                    attacker.CharacterID, target.CharacterID, force));
            }
        }

        private void OnAttackStarted(OnAttackStartedEvent evt)
        {
            if (evt.Type != AttackType.Simple)
                return;

            if (!_states.TryGetValue(evt.CharacterID, out var state))
                return;

            state.IsActive = true;
            state.Elapsed = 0f;
        }

        private void OnAttackEnded(OnAttackEndedEvent evt)
        {
            if (evt.Type != AttackType.Simple)
                return;

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
                _states[characterID] = new SimpleAttackHitDetectionState();
        }

        private void Unregister(Guid characterID)
            => _states.Remove(characterID);

        public void Dispose()
        {
            _gameEventsBus.Unsubscribe<OnAttackStartedEvent>(OnAttackStarted);
            _gameEventsBus.Unsubscribe<OnAttackEndedEvent>(OnAttackEnded);

            _context.OnCharacterAdded   -= Register;
            _context.OnCharacterRemoved -= Unregister;
        }
    }
}

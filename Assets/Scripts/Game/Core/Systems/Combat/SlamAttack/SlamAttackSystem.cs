using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.GameEvents;
using UnityEngine;
using Zenject;

namespace Game.Core.Systems
{
    public sealed class SlamAttackSystem : ISlamAttackSystem, IFixedTickable
    {
        private const float CMinimumSqrMagnitude = 0.0001f;

        private readonly GameEventsBus     _events;
        private readonly ICharacterContext _context;
        private readonly Dictionary<Guid, SlamAttackState> _states = new();

        public SlamAttackSystem(
            GameEventsBus     events,
            ICharacterContext context
        )
        {
            _events  = events;
            _context = context;

            _events.Subscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);
            _events.Subscribe<OnPowerAttackEndedEvent>(OnPowerAttackEnded);

            _context.OnCharacterAdded   += Register;
            _context.OnCharacterRemoved += Unregister;

            RegisterExistingCharacters();
        }

        private void RegisterExistingCharacters()
        {
            var characters = _context.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                Register(characters[i].CharacterID);
            }
        }

        private void Register(Guid characterId)
        {
            if (!_states.ContainsKey(characterId))
                _states[characterId] = new SlamAttackState();
        }

        private void Unregister(Guid characterId)
            => _states.Remove(characterId);

        private void OnPowerAttackStarted(OnPowerAttackStartedEvent evt)
        {
            ICharacterModel attacker = _context.GetModel(evt.CharacterID);
            if (attacker == null || !attacker.Enabled ||
                !_states.TryGetValue(evt.CharacterID, out SlamAttackState state))
            {
                return;
            }

            state.Begin();

            _events.Publish(new OnForceEvent(
                attacker.CharacterID, CalculateLaunchVelocity(attacker)));

            if (attacker.Data.Combat.PowerAttack.WaveStartTime <= 0f)
            {
                ApplyWave(attacker);
                state.MarkWaveApplied();
            }
        }

        private void OnPowerAttackEnded(OnPowerAttackEndedEvent evt)
        {
            if (_states.TryGetValue(evt.CharacterID, out SlamAttackState state) && state.IsActive)
                state.End();            
        }

        public void FixedTick()
        {
            var characters = _context.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                ICharacterModel attacker = characters[i];
                if (!_states.TryGetValue(attacker.CharacterID, out SlamAttackState state) || !state.IsActive)
                    continue;

                if (!attacker.Enabled)
                {
                    state.End();
                    continue;
                }

                UpdateSlam(attacker, state);
            }
        }

        private void UpdateSlam(ICharacterModel attacker, SlamAttackState state)
        {
            PowerAttackSettings settings = attacker.Data.Combat.PowerAttack;
            if (settings == null)
            {
                state.End();
                return;
            }

            state.Elapsed += Time.fixedDeltaTime;

            if (!state.IsWaveApplied && state.Elapsed >= settings.WaveStartTime)
            {
                ApplyWave(attacker);
                state.MarkWaveApplied();
            }
        }

        private static Vector3 CalculateLaunchVelocity(ICharacterModel attacker)
        {
            PowerAttackSettings settings = attacker.Data.Combat.PowerAttack;
            Vector3 direction            = attacker.Forward;
            direction.y = 0f;

            if (direction.sqrMagnitude <= CMinimumSqrMagnitude)
                return Vector3.zero;

            direction.Normalize();
            
            float gravity = Mathf.Abs(Physics.gravity.y);
            if (gravity <= Mathf.Epsilon)
                return direction * settings.Distance;

            float verticalVelocity   = Mathf.Sqrt(2f * gravity * settings.JumpHeight);
            float flightTime         = 2f * verticalVelocity / gravity;
            float horizontalVelocity = settings.Distance / flightTime;

            return direction * horizontalVelocity + Vector3.up * verticalVelocity;
        }

        private void ApplyWave(ICharacterModel attacker)
        {
            Vector3 attackerPosition     = attacker.Position;
            PowerAttackSettings settings = attacker.Data.Combat.PowerAttack;

            float radiusSqr = settings.WaveRadius * settings.WaveRadius;
            
            var characters = _context.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                ICharacterModel target = characters[i];
                if (target.CharacterID == attacker.CharacterID || !target.Enabled)
                    continue;

                Vector3 hitDirection = target.Position - attackerPosition;
                if (Mathf.Abs(hitDirection.y) > settings.WaveHeight)
                    continue;

                hitDirection.y = 0f;
                if (hitDirection.sqrMagnitude > radiusSqr)
                    continue;

                if (hitDirection.sqrMagnitude <= CMinimumSqrMagnitude)
                {
                    hitDirection   = attacker.Forward;
                    hitDirection.y = 0f;
                }

                hitDirection.Normalize();

                _events.Publish(new OnHitResolvedEvent(new HitData(
                    AttackType.Power, attacker.CharacterID, target.CharacterID, hitDirection)));
            }
        }

        public void Dispose()
        {
            _events.Unsubscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);
            _events.Unsubscribe<OnPowerAttackEndedEvent>(OnPowerAttackEnded);

            _context.OnCharacterAdded   -= Register;
            _context.OnCharacterRemoved -= Unregister;
        }
    }
}

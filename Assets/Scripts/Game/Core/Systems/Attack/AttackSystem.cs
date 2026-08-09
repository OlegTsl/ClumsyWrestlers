using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.GameEvents;
using UnityEngine;
using Zenject;

namespace Game.Core.Systems
{
    public class AttackSystem : IAttackSystem, IFixedTickable
    {
        private readonly GameEventsBus     _gameEventsBus;
        private readonly ICharacterContext _context;
        private readonly Dictionary<Guid, AttackState> _states = new();

        public AttackSystem(
            GameEventsBus     gameEventsBus,
            ICharacterContext context
        )
        {
            _gameEventsBus = gameEventsBus;
            _context       = context;
            
            _gameEventsBus.Subscribe<OnAttackEvent>(OnAttack);

            _context.OnCharacterAdded   += Register;
            _context.OnCharacterRemoved += Unregister;
        }

        private void Register(Guid id)
            => _states[id] = new AttackState();

        private void Unregister(Guid id)
            => _states.Remove(id);

        private bool CanAttack(Guid characterID, out AttackState state)
        {
            state = null;
            
            var model = _context.GetModel(characterID);
            if (model == null || !model.Enabled)
                return false;

            if (!_states.TryGetValue(characterID, out state))
                return false;

            return !state.SimpleAttackStarted && !state.PowerAttackStarted;
        }

        private void OnAttack(OnAttackEvent evt)
        {
            switch (evt.Type)
            {
                case AttackType.Simple:
                    OnSimpleAttack(evt.CharacterID);
                    break;
                case AttackType.Power:
                    OnPowerAttack(evt.CharacterID);
                    break;
                default:
                    break;
            }
        }

        private void OnSimpleAttack(Guid characterID)
        {
            if (CanAttack(characterID, out var state))
                StartSimpleAttack(characterID, state);
        }

        private void OnPowerAttack(Guid characterID)
        {
            if (CanAttack(characterID, out var state))
                StartPowerAttack(characterID, state);
        }

        public void FixedTick()
        {
            foreach (var (id, state) in _states)
            {
                var model = _context.GetModel(id);
                if (model == null || !model.Enabled)
                    continue;

                if (state.SimpleAttackStarted)
                    HandleSimpleAttack(model, state);

                if (state.PowerAttackStarted)
                    HandlePowerAttack(model, state);
            }
        }

        private void StartSimpleAttack(Guid characterID, AttackState state)
        {
            state.SimpleAttackStarted = true;
            state.AttackElapsed       = 0f;

            _gameEventsBus.Publish(new OnAttackStartedEvent(
                characterID, AttackType.Simple));
        }

        private void EndSimpleAttack(ICharacterModel model, AttackState state)
        {
            model.SetHitsEnabled(false);
            state.SimpleAttackStarted = false;

            _gameEventsBus.Publish(new OnAttackEndedEvent(
                model.CharacterID, AttackType.Simple));
        }

        private void HandleSimpleAttack(ICharacterModel model, AttackState state)
        {
            var settings = model.Data.Combat;
            state.AttackElapsed += Time.fixedDeltaTime;

            bool enabled = state.AttackElapsed >= settings.SimpleAttackHitboxStart
                        && state.AttackElapsed <  settings.SimpleAttackHitboxEnd;

            model.SetHitsEnabled(enabled);

            if (state.AttackElapsed >= settings.SimpleAttackDuration)
                EndSimpleAttack(model, state);
        }

        private void StartPowerAttack(Guid characterID, AttackState state)
        {
            state.PowerAttackStarted = true;
            state.AttackElapsed      = 0f;
            state.WaveApplied        = false;

            _gameEventsBus.Publish(new OnAttackStartedEvent(
                characterID, AttackType.Power));
        }

        private void EndPowerAttack(ICharacterModel model, AttackState state)
        {
            model.SetHitsEnabled(false);
            state.PowerAttackStarted = false;

            _gameEventsBus.Publish(new OnAttackEndedEvent(
                model.CharacterID, AttackType.Power));
        }

        private void HandlePowerAttack(ICharacterModel model, AttackState state)
        {
            if (state.AttackElapsed == 0f)
            {
                Vector3 force = CalculateForce(model);
                _gameEventsBus.Publish(new OnForceEvent(model.CharacterID, force, false));
            }

            if (!state.WaveApplied && state.AttackElapsed >= model.Data.Combat.PowerAttackWaveStart)
            {
                state.WaveApplied = true;
                ApplyPowerWave(model);
            }

            state.AttackElapsed += Time.fixedDeltaTime;

            if (state.AttackElapsed >= model.Data.Combat.PowerAttackDuration)
                EndPowerAttack(model, state);
        }

        private Vector3 CalculateForce(ICharacterModel model)
        {
            Vector3 direction = model.Forward;
            direction.y = 0;
            direction.Normalize();

            float gravity          = Physics.gravity.y;
            float verticalVelocity = Mathf.Sqrt(-2f * gravity * model.Data.Combat.PowerAttackJumpHeight);
            float totalTime        = 2f * verticalVelocity                 / Mathf.Abs(gravity);
            float horizontalSpeed  = model.Data.Combat.PowerAttackDistance / totalTime;

            return direction * horizontalSpeed + Vector3.up * verticalVelocity;
        }

        private void ApplyPowerWave(ICharacterModel attacker)
        {
            Vector3 attackerPos = attacker.Transform.position;
            var settings = attacker.Data.Combat;

            foreach (var (id, _) in _states)
            {
                if (id == attacker.CharacterID)
                    continue;

                var target = _context.GetModel(id);
                if (target == null || !target.Enabled)
                    continue;

                Vector3 targetPos = target.Transform.position;
                float horizontalDist = Vector3.Distance(
                    new Vector3(targetPos.x,   0, targetPos.z),
                    new Vector3(attackerPos.x, 0, attackerPos.z)
                );

                if (horizontalDist > settings.PowerWaveRadius)
                    continue;

                if (targetPos.y - attackerPos.y > settings.PowerWaveHeight)
                    continue;

                _gameEventsBus.Publish(new OnApplyDamageEvent(
                    attacker.CharacterID, target.CharacterID, settings.PowerAttackForce));
            }
        }

        public void Dispose()
        {
            _gameEventsBus.Unsubscribe<OnAttackEvent>(OnAttack);

            _context.OnCharacterAdded   -= Register;
            _context.OnCharacterRemoved -= Unregister;
        }
    }
}
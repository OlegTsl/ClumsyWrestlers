using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.GameEvents;
using UnityEngine;
using Zenject;

namespace Game.Core.Systems
{
    public sealed class PowerAttackSystem : IPowerAttackSystem, IFixedTickable
    {
        private readonly GameEventsBus     _events;
        private readonly ICharacterContext _context;
        private readonly Dictionary<Guid, PowerAttackState> _states = new();

        public PowerAttackSystem(
            GameEventsBus     events,
            ICharacterContext context
        )
        {
            _events  = events;
            _context = context;

            _events.Subscribe<OnPowerAttackRequestedEvent>(OnPowerAttackRequested);
            _events.Subscribe<OnHitResolvedEvent>(OnHitResolved);

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
                _states[characterId] = new PowerAttackState();
        }

        private void Unregister(Guid characterId)
        {
            if (_states.TryGetValue(characterId, out PowerAttackState state) && state.IsAttacking)
            {
                ICharacterModel model = _context.GetModel(characterId);
                model?.SetMovable(true);
            }

            _states.Remove(characterId);
        }

        private void OnPowerAttackRequested(OnPowerAttackRequestedEvent evt)
        {
            ICharacterModel model = _context.GetModel(evt.CharacterID);
            if (model == null || !model.Enabled)
                return;

            if (!_states.TryGetValue(evt.CharacterID, out PowerAttackState state) || state.IsAttacking)
                return;

            StartPowerAttack(model, state);
        }

        private void OnHitResolved(OnHitResolvedEvent evt)
        {
            if (!_states.TryGetValue(evt.Hit.TargetID, out PowerAttackState state) || !state.IsAttacking)
                return;

            ICharacterModel model = _context.GetModel(evt.Hit.TargetID);
            if (model != null)
                EndPowerAttack(model, state, PowerAttackEndReason.Interrupted);
        }

        public void FixedTick()
        {
            var characters = _context.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                ICharacterModel model = characters[i];
                if (!_states.TryGetValue(model.CharacterID, out PowerAttackState state) || !state.IsAttacking)
                    continue;

                if (!model.Enabled)
                {
                    EndPowerAttack(model, state, PowerAttackEndReason.Interrupted);
                    continue;
                }

                UpdatePowerAttack(model, state);
            }
        }

        private void StartPowerAttack(ICharacterModel model, PowerAttackState state)
        {
            PowerAttackSettings settings = model.Data.Combat.PowerAttack;
            if (settings.Duration <= 0f || settings.AnimationClip == null)
                return;

            state.StartAttack();
            model.SetMovable(false);

            _events.Publish(new OnPowerAttackStartedEvent(
                model.CharacterID, settings.AnimationClip));
        }

        private void UpdatePowerAttack(ICharacterModel model, PowerAttackState state)
        {
            PowerAttackSettings settings = model.Data.Combat.PowerAttack;
            if (settings == null || settings.Duration <= 0f)
            {
                EndPowerAttack(model, state, PowerAttackEndReason.Interrupted);
                return;
            }

            state.Elapsed += Time.fixedDeltaTime;
            if (state.Elapsed >= settings.Duration)
                EndPowerAttack(model, state, PowerAttackEndReason.Completed);
        }

        private void EndPowerAttack(ICharacterModel model, PowerAttackState state, PowerAttackEndReason reason)
        {
            if (!state.IsAttacking)
                return;

            state.EndAttack();
            model.SetMovable(true);

            _events.Publish(new OnPowerAttackEndedEvent(
                model.CharacterID, reason));
        }

        public void Dispose()
        {
            _events.Unsubscribe<OnPowerAttackRequestedEvent>(OnPowerAttackRequested);
            _events.Unsubscribe<OnHitResolvedEvent>(OnHitResolved);

            _context.OnCharacterAdded   -= Register;
            _context.OnCharacterRemoved -= Unregister;
        }
    }
}

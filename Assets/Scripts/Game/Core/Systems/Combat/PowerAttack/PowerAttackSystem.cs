using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.Entities;
using Game.Core.GameEvents;
using UnityEngine;
using Zenject;

namespace Game.Core.Systems
{
    public sealed class PowerAttackSystem : IPowerAttackSystem, IFixedTickable
    {
        private readonly IGameEventsBus     _events;
        private readonly ICharacterContext _context;
        private readonly Dictionary<EntityId, PowerAttackState> _states = new(16);

        public PowerAttackSystem(
            IGameEventsBus     events,
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

        private void Register(EntityId characterId)
        {
            if (!_states.ContainsKey(characterId))
                _states[characterId] = new PowerAttackState();
        }

        private void Unregister(EntityId characterId)
        {
            if (_states.TryGetValue(characterId, out PowerAttackState state) && state.IsAttacking)
            {
                ICharacterModel model = _context.GetModel(characterId);
                model?.GetState<ICharacterMovementState>().SetMovable(true);
            }

            _states.Remove(characterId);
        }

        private void OnPowerAttackRequested(OnPowerAttackRequestedEvent evt)
        {
            ICharacterModel model = _context.GetModel(evt.CharacterID);
            if (model == null)
                return;

            ICharacterCombatRuntimeState combat =
                model.GetState<ICharacterCombatRuntimeState>();
            if (!combat.Enabled)
                return;

            if (!_states.TryGetValue(evt.CharacterID, out PowerAttackState state) || state.IsAttacking)
                return;

            StartPowerAttack(combat, state);
        }

        private void OnHitResolved(OnHitResolvedEvent evt)
        {
            if (evt.Hit.TargetType != HitObjectType.Character)
                return;

            if (!_states.TryGetValue(evt.Hit.TargetID, out PowerAttackState state) || !state.IsAttacking)
                return;

            ICharacterModel model = _context.GetModel(evt.Hit.TargetID);
            if (model != null)
                EndPowerAttack(
                    model.GetState<ICharacterCombatRuntimeState>(),
                    state);
        }

        public void FixedTick()
        {
            var characters = _context.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                ICharacterModel model = characters[i];
                if (!_states.TryGetValue(model.CharacterID, out PowerAttackState state) || !state.IsAttacking)
                    continue;

                ICharacterCombatRuntimeState combat =
                    model.GetState<ICharacterCombatRuntimeState>();
                if (!combat.Enabled)
                {
                    EndPowerAttack(combat, state);
                    continue;
                }

                UpdatePowerAttack(combat, state);
            }
        }

        private void StartPowerAttack(
            ICharacterCombatRuntimeState model,
            PowerAttackState state)
        {
            PowerAttackSettings settings = model.Data.Combat.PowerAttack;
            if (settings.Duration <= 0f || settings.AnimationClip == null)
                return;

            state.StartAttack();
            model.SetMovable(false);

            _events.Publish(new OnPowerAttackStartedEvent(
                model.CharacterID, settings.AnimationClip));
        }

        private void UpdatePowerAttack(
            ICharacterCombatRuntimeState model,
            PowerAttackState state)
        {
            state.Elapsed += Time.fixedDeltaTime;

            if (state.Elapsed >= model.Data.Combat.PowerAttack.Duration)
                EndPowerAttack(model, state);
        }

        private void EndPowerAttack(
            ICharacterCombatRuntimeState model,
            PowerAttackState state)
        {
            if (!state.IsAttacking)
                return;

            state.EndAttack();
            model.SetMovable(true);

            _events.Publish(new OnPowerAttackEndedEvent(
                model.CharacterID));
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

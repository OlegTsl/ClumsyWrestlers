using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.Entities;
using Game.Core.GameEvents;
using UnityEngine;
using Zenject;

namespace Game.Core.Systems
{
    public sealed class SimpleAttackSystem : ISimpleAttackSystem, IFixedTickable
    {
        private readonly IGameEventsBus     _gameEventsBus;
        private readonly ICharacterContext _context;
        private readonly Dictionary<EntityId, SimpleAttackState> _states = new(16);

        public SimpleAttackSystem(
            IGameEventsBus     gameEventsBus,
            ICharacterContext context
        )
        {
            _gameEventsBus = gameEventsBus;
            _context       = context;

            _gameEventsBus.Subscribe<OnSimpleAttackInputEvent>(OnAttackInput);
            _gameEventsBus.Subscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);
            _gameEventsBus.Subscribe<OnHitResolvedEvent>(OnHitResolved);

            _context.OnCharacterAdded   += Register;
            _context.OnCharacterRemoved += Unregister;

            RegisterExistingCharacters();
        }

        public void FixedTick()
        {
            foreach (var pair in _states)
            {
                ICharacterModel   model = _context.GetModel(pair.Key);
                SimpleAttackState state = pair.Value;

                if (model == null)
                {
                    InterruptAttack(model, state);
                    continue;
                }

                ICharacterCombatRuntimeState combat =
                    model.GetState<ICharacterCombatRuntimeState>();
                if (!combat.Enabled)
                {
                    InterruptAttack(model, state);
                    continue;
                }

                if (!state.IsAttacking)
                    continue;

                UpdateAttack(combat, state);
            }
        }

        private void OnAttackInput(OnSimpleAttackInputEvent evt)
        {
            if (!_states.TryGetValue(evt.CharacterID, out var state))
                return;

            state.IsInputPressed = evt.IsPressed;

            if (!evt.IsPressed)
                return;

            ICharacterModel model = _context.GetModel(evt.CharacterID);
            if (model == null)
                return;

            ICharacterCombatRuntimeState combat =
                model.GetState<ICharacterCombatRuntimeState>();
            if (!combat.Enabled || !combat.IsMovable)
                return;

            if (state.IsAttacking)
                return;

            StartAttack(combat, state);
        }

        private void UpdateAttack(
            ICharacterCombatRuntimeState model,
            SimpleAttackState state)
        {
            state.Elapsed += Time.fixedDeltaTime;
            UpdateHandIk(model, state);

            if (state.Elapsed < model.Data.Combat.SimpleAttack.Duration)
                return;

            EndAttack(model, state);
            state.ToggleHand();

            if (state.IsInputPressed && model.IsMovable)
                StartAttack(model, state);
        }

        private void UpdateHandIk(
            ICharacterCombatRuntimeState model,
            SimpleAttackState state)
        {
            SimpleAttackSettings settings = model.Data.Combat.SimpleAttack;

            float normalizedTime = state.Elapsed / settings.Duration;
            float windowProgress = Mathf.InverseLerp(settings.HitboxStartNormalized,
                settings.HitboxEndNormalized, normalizedTime);

            float blend  = Mathf.SmoothStep(0f, 1f, Mathf.Sin(windowProgress * Mathf.PI));
            float weight = blend * settings.HandIkWeight;

            Vector3 targetPosition = model.AttackOrigin + model.Forward * settings.HandIkReach;
            _gameEventsBus.Publish(new OnAttackHandIkEvent(
                model.CharacterID,
                state.ActiveHand,
                targetPosition,
                weight));
        }

        private void StartAttack(
            ICharacterCombatRuntimeState model,
            SimpleAttackState state)
        {
            SimpleAttackSettings settings = model.Data.Combat.SimpleAttack;
            if (settings == null || settings.Duration <= 0f)
                return;

            state.StartAttack();

            _gameEventsBus.Publish(new OnSimpleAttackStartedEvent(
                model.CharacterID, state.IsMirrored));
        }

        private void EndAttack(
            ICharacterCombatRuntimeState model,
            SimpleAttackState state)
        {
            _gameEventsBus.Publish(new OnAttackHandIkClearedEvent(model.CharacterID));
            state.FinishAttack();

            _gameEventsBus.Publish(
                new OnSimpleAttackEndedEvent(model.CharacterID));
        }

        private void OnPowerAttackStarted(OnPowerAttackStartedEvent evt)
        {
            if (_states.TryGetValue(evt.CharacterID, out var state))
                InterruptAttack(_context.GetModel(evt.CharacterID), state);
        }

        private void OnHitResolved(OnHitResolvedEvent evt)
        {
            if (evt.Hit.TargetType != HitObjectType.Character)
                return;

            if (_states.TryGetValue(evt.Hit.TargetID, out var state))
                InterruptAttack(_context.GetModel(evt.Hit.TargetID), state);
        }

        private void InterruptAttack(ICharacterModel model, SimpleAttackState state)
        {
            if (state.IsAttacking && model != null)
            {
                _gameEventsBus.Publish(
                    new OnSimpleAttackEndedEvent(model.CharacterID));

                _gameEventsBus.Publish(new OnAttackHandIkClearedEvent(model.CharacterID));
            }

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

        private void Register(EntityId characterID)
        {
            if (!_states.ContainsKey(characterID))
                _states[characterID] = new SimpleAttackState();
        }

        private void Unregister(EntityId characterID)
            => _states.Remove(characterID);

        public void Dispose()
        {
            _gameEventsBus.Unsubscribe<OnSimpleAttackInputEvent>(OnAttackInput);
            _gameEventsBus.Unsubscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);
            _gameEventsBus.Unsubscribe<OnHitResolvedEvent>(OnHitResolved);

            _context.OnCharacterAdded   -= Register;
            _context.OnCharacterRemoved -= Unregister;
        }
    }
}

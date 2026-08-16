using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.GameEvents;
using UnityEngine;
using Zenject;

namespace Game.Core.Systems
{
    public sealed class SimpleAttackSystem : ISimpleAttackSystem, IFixedTickable
    {
        private readonly GameEventsBus     _gameEventsBus;
        private readonly ICharacterContext _context;
        private readonly Dictionary<Guid, SimpleAttackState> _states = new();

        public SimpleAttackSystem(
            GameEventsBus     gameEventsBus,
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

                if (model == null || !model.Enabled)
                {
                    InterruptAttack(model, state);
                    continue;
                }

                if (!state.IsAttacking)
                    continue;

                UpdateAttack(model, state);
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
            if (model == null || !model.Enabled || !model.IsMovable)
                return;

            if (state.IsAttacking)
                return;

            StartAttack(model, state);
        }

        private void UpdateAttack(ICharacterModel model, SimpleAttackState state)
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

        private static void UpdateHandIk(ICharacterModel model, SimpleAttackState state)
        {
            SimpleAttackSettings settings = model.Data.Combat.SimpleAttack;

            float normalizedTime = state.Elapsed / settings.Duration;
            float windowProgress = Mathf.InverseLerp(settings.HitboxStartNormalized,
                settings.HitboxEndNormalized, normalizedTime);

            float blend  = Mathf.SmoothStep(0f, 1f, Mathf.Sin(windowProgress * Mathf.PI));
            float weight = blend * settings.HandIkWeight;

            Vector3 targetPosition = model.AttackOrigin + model.Forward * settings.HandIkReach;
            model.SetAttackHandIk(state.ActiveHand, targetPosition, weight);
        }

        private void StartAttack(ICharacterModel model, SimpleAttackState state)
        {
            SimpleAttackSettings settings = model.Data.Combat.SimpleAttack;
            if (settings == null || settings.Duration <= 0f)
                return;

            state.StartAttack();

            _gameEventsBus.Publish(new OnSimpleAttackStartedEvent(
                model.CharacterID, state.IsMirrored));
        }

        private void EndAttack(ICharacterModel model, SimpleAttackState state)
        {
            model.ClearAttackHandIk();
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

                model.ClearAttackHandIk();
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

        private void Register(Guid characterID)
        {
            if (!_states.ContainsKey(characterID))
                _states[characterID] = new SimpleAttackState();
        }

        private void Unregister(Guid characterID)
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

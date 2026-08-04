using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.GameEvents;
using UnityEngine;
using Zenject;

namespace Game.Core.Systems
{
    public class SimpleAttackSystem : ISimpleAttackSystem, IFixedTickable
    {
        private readonly GameEventsBus     _gameEventsBus;
        private readonly ICharacterContext _context;
        private readonly Dictionary<Guid, AttackState> _states = new();

        public SimpleAttackSystem(
            GameEventsBus     gameEventsBus,
            ICharacterContext context
        )
        {
            _gameEventsBus = gameEventsBus;
            _context       = context;
            
            _gameEventsBus.Subscribe<OnSimpleAttackEvent>(OnSimpleAttack);

            _context.OnCharacterAdded   += Register;
            _context.OnCharacterRemoved += Unregister;
        }

        private void Register(Guid id)
            => _states[id] = new AttackState();

        private void Unregister(Guid id)
            => _states.Remove(id);

        private void OnSimpleAttack(OnSimpleAttackEvent evt)
        {
            var model = _context.GetModel(evt.CharacterID);
            if (model == null || !model.Enabled)
                return;

            if (!_states.TryGetValue(evt.CharacterID, out var state))
                return;

            if (state.AttackStarted)
                return;

            StartSimpleAttack(evt.CharacterID, state);
        }

        public void FixedTick()
        {
            foreach (var (id, state) in _states)
            {
                var model = _context.GetModel(id);
                if (model == null || !model.Enabled)
                    continue;

                if (state.AttackStarted)
                    HandleSimpleAttack(model, state);
            }
        }

        private void StartSimpleAttack(Guid characterID, AttackState state)
        {
            state.AttackStarted = true;
            state.AttackElapsed = 0f;

            _gameEventsBus.Publish(new OnSimpleAttackStartedEvent(characterID));
        }

        private void EndSimpleAttack(ICharacterModel model, AttackState state)
        {
            model.SetHitsEnabled(false);
            state.AttackStarted = false;

            _gameEventsBus.Publish(new OnSimpleAttackEndedEvent(model.CharacterID));
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

        public void Dispose()
        {
            _gameEventsBus.Unsubscribe<OnSimpleAttackEvent>(OnSimpleAttack);

            _context.OnCharacterAdded   -= Register;
            _context.OnCharacterRemoved -= Unregister;
        }
    }
}
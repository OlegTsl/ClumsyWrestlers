using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.GameEvents;
using UnityEngine;
using Zenject;

namespace Game.Core.Combat
{
    public class SimpleAttackController : ISimpleAttackController, IFixedTickable
    {
        private readonly GameEventsBus     _gameEventsBus;
        private readonly ICharacterContext _context;
        private readonly Dictionary<Guid, SimpleAttackState> _states = new();

        public SimpleAttackController(
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
            => _states[id] = new SimpleAttackState();

        private void Unregister(Guid id)
            => _states.Remove(id);

        private void OnSimpleAttack(OnSimpleAttackEvent evt)
        {
            var character = _context.GetCharacter(evt.CharacterID);
            if (character == null || !character.Enabled)
                return;

            if (!_states.TryGetValue(evt.CharacterID, out var state))
                return;

            if (state.IsSimpleAttack || state.IsPowerAttack)
                return;

            StartSimpleAttack(evt.CharacterID, state);
        }

        public void FixedTick()
        {
            foreach (var (id, state) in _states)
            {
                var character = _context.GetCharacter(id);
                if (character == null || !character.Enabled)
                    continue;

                if (state.IsSimpleAttack)
                {
                    var settings = character.Data.CombatSettings;
                    HandleSimpleAttack(id, settings, state);
                }
            }
        }

        private void StartSimpleAttack(Guid characterID, SimpleAttackState state)
        {
            state.IsSimpleAttack = true;
            state.AttackElapsed  = 0f;

            _gameEventsBus.Publish(new OnSimpleAttackStartedEvent(characterID));
        }

        private void EndSimpleAttack(Guid characterID, SimpleAttackState state)
        {
            SetHitboxActive(characterID, state, false);
            state.IsSimpleAttack = false;

            _gameEventsBus.Publish(new OnSimpleAttackEndedEvent(characterID));
        }

        private void HandleSimpleAttack(
            Guid              characterID,
            CombatSettings    settings,
            SimpleAttackState state
        )
        {
            state.AttackElapsed += Time.fixedDeltaTime;

            bool active = state.AttackElapsed >= settings.SimpleAttackHitboxStart
                       && state.AttackElapsed <  settings.SimpleAttackHitboxEnd;

            SetHitboxActive(characterID, state, active);

            if (state.AttackElapsed >= settings.SimpleAttackDuration)
                EndSimpleAttack(characterID, state);
        }

        private void SetHitboxActive(
            Guid              characterID,
            SimpleAttackState state,
            bool              active
        )
        {
            if (state.IsHitboxActive == active)
                return;

            state.IsHitboxActive = active;
            _gameEventsBus.Publish(new OnHitboxEnabledEvent(characterID, active));
        }

        public void Dispose()
        {
            _gameEventsBus.Unsubscribe<OnSimpleAttackEvent>(OnSimpleAttack);

            _context.OnCharacterAdded   -= Register;
            _context.OnCharacterRemoved -= Unregister;
        }
    }
}
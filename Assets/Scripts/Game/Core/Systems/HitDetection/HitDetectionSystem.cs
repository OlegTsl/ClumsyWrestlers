using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.GameEvents;
using UnityEngine;

namespace Game.Core.Systems
{
    public class HitDetectionSystem : IHitDetectionSystem
    {
        private readonly GameEventsBus     _gameEventsBus;
        private readonly ICharacterContext _context;
        private readonly Dictionary<Guid, HitDetectionState> _states = new();

        public HitDetectionSystem(
            GameEventsBus     gameEventsBus,
            ICharacterContext context)
        {
            _gameEventsBus = gameEventsBus;
            _context       = context;
            
            _gameEventsBus.Subscribe<OnHitboxEnabledEvent>(OnHitBoxEnabled);

            _context.OnCharacterAdded   += Register;
            _context.OnCharacterRemoved += Unregister;
        }

        private void Register(Guid characterID)
            => _states[characterID] = new HitDetectionState();

        private void Unregister(Guid characterID)
            => _states.Remove(characterID);

        private void OnHitBoxEnabled(OnHitboxEnabledEvent evt)
        {
            if (!_states.TryGetValue(evt.CharacterID, out var state))
                return;

            state.Enabled = evt.Enabled;
            state.Targets.Clear();
        }

        public void OnTriggerEnter(Guid characterID, Collider other)
        {
            if (!_states.TryGetValue(characterID, out var state))
                return;

            if (!state.Enabled || state.Targets.Contains(other))
                return;

            var target = _context.GetCharacter(other);
            if (target == null)
                return;

            state.Targets.Add(other);
            //_gameEventsBus.Publish(new OnHitLandedEvent(targetCharacter.CharacterID));
        }

        public void Dispose()
        {
            _gameEventsBus.Unsubscribe<OnHitboxEnabledEvent>(OnHitBoxEnabled);

            _context.OnCharacterAdded   -= Register;
            _context.OnCharacterRemoved -= Unregister;
        }
    }
}
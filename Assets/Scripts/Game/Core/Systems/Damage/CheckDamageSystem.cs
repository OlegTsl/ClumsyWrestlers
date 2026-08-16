using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.GameEvents;

namespace Game.Core.Systems
{
    public class CheckDamageSystem : ICheckDamageSystem
    {
        private const int InitialTargetCapacity = 8;

        private readonly GameEventsBus     _gameEventsBus;
        private readonly ICharacterContext _context;
        private readonly Dictionary<Guid, List<Guid>> _hitTargets = new();

        public CheckDamageSystem(
            GameEventsBus     events,
            ICharacterContext context
        )
        {
            _gameEventsBus = events;
            _context       = context;

            _gameEventsBus.Subscribe<OnHitDetectedEvent>(OnHitDetected);
            _gameEventsBus.Subscribe<OnSimpleAttackStartedEvent>(OnSimpleAttackStarted);

            _context.OnCharacterAdded   += Register;
            _context.OnCharacterRemoved += Unregister;

            RegisterExistingCharacters();
        }

        private void OnSimpleAttackStarted(OnSimpleAttackStartedEvent evt)
            => ClearTargets(evt.CharacterID);

        private void ClearTargets(Guid characterID)
        {
            if (_hitTargets.TryGetValue(characterID, out var targets))
                targets.Clear();
        }

        private void OnHitDetected(OnHitDetectedEvent evt)
        {
            HitData hit = evt.Hit;
            
            var target   = _context.GetModel(hit.TargetID);
            var attacker = _context.GetModel(hit.AttackerID);

            if (target == null  || attacker == null ||
                !target.Enabled || !attacker.Enabled)
            {
                return;
            }

            if (!_hitTargets.TryGetValue(hit.AttackerID, out var targets))
                return;

            if (targets.Contains(hit.TargetID))
                return;

            targets.Add(target.CharacterID);
            _gameEventsBus.Publish(new OnHitResolvedEvent(hit));
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
            if (!_hitTargets.ContainsKey(characterID))
                _hitTargets[characterID] = new List<Guid>(InitialTargetCapacity);
        }

        private void Unregister(Guid characterID)
            => _hitTargets.Remove(characterID);

        public void Dispose()
        {
            _gameEventsBus.Unsubscribe<OnHitDetectedEvent>(OnHitDetected);
            _gameEventsBus.Unsubscribe<OnSimpleAttackStartedEvent>(OnSimpleAttackStarted);

            _context.OnCharacterAdded   -= Register;
            _context.OnCharacterRemoved -= Unregister;
        }
    }
}

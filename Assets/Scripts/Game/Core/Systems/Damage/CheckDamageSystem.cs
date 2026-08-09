using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.GameEvents;

namespace Game.Core.Systems
{
    public class CheckDamageSystem : ICheckDamageSystem
    {
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
            
            _gameEventsBus.Subscribe<OnDamageEvent>(OnDamage);
            _gameEventsBus.Subscribe<OnAttackStartedEvent>(OnAttackStarted);
            _gameEventsBus.Subscribe<OnAttackEndedEvent>(OnAttackEnded);

            _context.OnCharacterAdded   += Register;
            _context.OnCharacterRemoved += Unregister;
        }

        private void OnAttackStarted(OnAttackStartedEvent evt)
            => ClearTargets(evt.CharacterID);

        private void OnAttackEnded(OnAttackEndedEvent evt)
            => ClearTargets(evt.CharacterID);

        private void ClearTargets(Guid characterID)
        {
            if (_hitTargets.TryGetValue(characterID, out var targets))
                targets.Clear();
        }

        private void OnDamage(OnDamageEvent evt)
        {
            var target = _context.GetModel(evt.TargetID);
            if (target == null)
                return;

            if (!_hitTargets.TryGetValue(evt.AttackerID, out var targets))
                return;

            if (targets.Contains(evt.TargetID))
                return;

            targets.Add(target.CharacterID);
            _gameEventsBus.Publish(new OnApplyDamageEvent(
                evt.AttackerID, evt.TargetID, evt.Force));
        }

        public void Register(Guid characterID)
        {
            if (!_hitTargets.ContainsKey(characterID))
                _hitTargets[characterID] = new List<Guid>();
        }

        public void Unregister(Guid characterID)
            => _hitTargets.Remove(characterID);

        public void Dispose()
        {
            _gameEventsBus.Unsubscribe<OnDamageEvent>(OnDamage);
            _gameEventsBus.Unsubscribe<OnAttackStartedEvent>(OnAttackStarted);
            _gameEventsBus.Unsubscribe<OnAttackEndedEvent>(OnAttackEnded);

            _context.OnCharacterAdded   -= Register;
            _context.OnCharacterRemoved -= Unregister;
        }
    }
}
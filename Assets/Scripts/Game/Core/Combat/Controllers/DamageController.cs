using System;
using Game.Core.Character;
using Game.Core.Events;
using UnityEngine;

namespace Game.Core.Combat
{
    public class DamageController : IDamageController, IDisposable
    {
        private readonly AttackSettings      _settings;
        private readonly Transform           _transform;
        private readonly GameEventsBus       _gameEventsBus;
        private readonly ICharactersRegistry _charactersRegistry;
        
        public DamageController(
            Transform           transform,
            AttackSettings      settings,
            GameEventsBus       gameEventsBus,
            ICharactersRegistry charactersRegistry
        )
        {
            _transform          = transform;
            _settings           = settings;
            _gameEventsBus      = gameEventsBus;
            _charactersRegistry = charactersRegistry;

            _gameEventsBus.Subscribe<OnPunchLandedEvent>(ApplyDamage);
        }

        private void ApplyDamage(OnPunchLandedEvent evt)
        {
            var context = _charactersRegistry.GetContext(evt.CharacterID);
            if (context == null)
                return;

            Collider target   = context.View.HitBox;
            
            Vector3 direction = (target.transform.position - _transform.position).normalized;
            context.Movement.ApplyExternalForce(direction * _settings.SimpleAttackKnockback);
        }

        public void Dispose()
            => _gameEventsBus.Unsubscribe<OnPunchLandedEvent>(ApplyDamage);
    }
}
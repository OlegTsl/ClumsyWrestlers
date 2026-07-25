using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.Events;
using UnityEngine;

namespace Game.Core.Combat
{
    public class HitController : IHitController, IDisposable
    {
        private readonly ICharactersRegistry _charactersRegistry;
        private readonly GameEventsBus       _gameEventsBus;
        
        private List<Collider> _hitTargets = new();
        private bool           _isEnabled;

        public event Action<Collider> OnHit;

        public HitController(
            GameEventsBus       gameEventsBus,
            ICharactersRegistry charactersRegistry
        )
        {
            _charactersRegistry = charactersRegistry;
            _gameEventsBus      = gameEventsBus;

            _gameEventsBus.Subscribe<OnHitBoxEnabledEvent>(OnHitBoxEnabled);
        }

        private void OnHitBoxEnabled(OnHitBoxEnabledEvent evt)
        {
            if (evt.Enabled)
                Enable();
            else
                Disable();
        }

        public void Enable()
        {
            _hitTargets.Clear();
            _isEnabled = true;
        }

        public void Disable()
        {
            _isEnabled = false;
            _hitTargets.Clear();
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (!_isEnabled || _hitTargets.Contains(collider))
                return;

            var characterID = _charactersRegistry.GetByCollider(collider);
            if (characterID == Guid.Empty)
                return;

            _hitTargets.Add(collider);
            _gameEventsBus.Publish(new OnPunchLandedEvent(characterID));
        }

        public void Dispose()
            => _gameEventsBus.Unsubscribe<OnHitBoxEnabledEvent>(OnHitBoxEnabled);
    }
}
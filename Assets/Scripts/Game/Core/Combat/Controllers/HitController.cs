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
        
        private IReadOnlyList<Collider> _colliders;
        private List<Collider>          _hitTargets = new();

        private bool _isEnabled;

        public event Action<Collider> OnHit;

        public HitController(
            ICharactersRegistry charactersRegistry,
            GameEventsBus       gameEventsBus
        )
        {
            _charactersRegistry = charactersRegistry;
            _gameEventsBus      = gameEventsBus;

            _gameEventsBus.Subscribe<OnPunchStartedEvent>(OnPunchStarted);
            _gameEventsBus.Subscribe<OnPunchEndedEvent>(OnPunchEnded);
        }

        public void Initialize(IReadOnlyList<Collider> colliders)
        {
            _colliders = colliders;
            
            foreach (var collider in _colliders)
            {
                collider.isTrigger = true;
            }
        }

        private void OnPunchStarted(OnPunchStartedEvent evt)
            => Enable();

        private void OnPunchEnded(OnPunchEndedEvent evt)
            => Disable();

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
        {
            _gameEventsBus.Unsubscribe<OnPunchStartedEvent>(OnPunchStarted);
            _gameEventsBus.Unsubscribe<OnPunchEndedEvent>(OnPunchEnded);
        }
    }
}
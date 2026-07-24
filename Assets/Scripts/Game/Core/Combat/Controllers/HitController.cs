using System;
using System.Collections.Generic;
using Game.Core.Character;
using UnityEngine;

namespace Game.Core.Combat
{
    public class HitController : IHitController
    {
        private readonly ICharacterRegistry _characterRegistry;
        
        private IReadOnlyList<Collider> _colliders;
        private List<Collider>          _hitTargets = new();

        private bool _isEnabled;

        public event Action<Collider> OnHit;

        public HitController(ICharacterRegistry characterRegistry)
        {
            _characterRegistry = characterRegistry;
        }

        public void Initialize(IReadOnlyList<Collider> colliders)
        {
            _colliders = colliders;
            
            foreach (var collider in _colliders)
            {
                collider.isTrigger = true;
            }
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

            ICharacterView targetView = _characterRegistry.GetByCollider(collider);
            if (targetView == null || collider != targetView.HitBox)
                return;

            _hitTargets.Add(collider);
            OnHit?.Invoke(collider);
        }
    }
}
using Game.Core.Character;
using Game.Core.Movement;
using UnityEngine;

namespace Game.Core.Combat
{
    public class DamageController : IDamageController
    {
        private IMovementController _movementController;
        private AttackSettings      _settings;
        private Transform           _transform;

        public void Initialize(
            Transform           transform,
            AttackSettings      settings,
            IHitController      hitController,
            IMovementController movementController
        )
        {
            _transform          = transform;
            _settings           = settings;
            _movementController = movementController;
            
            hitController.OnHit += ApplyDamage;
        }

        private void ApplyDamage(Collider target)
        {
            Vector3 direction = (target.transform.position - _transform.position).normalized;
            _movementController.ApplyExternalForce(direction * _settings.SimpleAttackKnockback);

            Vector3 force = direction * _settings.SimpleAttackKnockback;
            Debug.Log($"ApplyDamage: target={target.name}, direction={direction}, force={force}");
        }
    }
}
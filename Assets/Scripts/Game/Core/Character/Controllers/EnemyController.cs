using Game.Core.Animation;
using Game.Core.Movement;
using UnityEngine;
using Zenject;

namespace Game.Core.Character
{
    public class EnemyController : IEnemyController, ILateTickable, IFixedTickable
    {
        private IMovementController  _movementController;
        private IAnimationController _animationController;
        private Rigidbody            _rigidbody;

        private bool _isEnabled = true;
        private bool _isInitialized;

        public void Initialize(ICharacterContext context)
        {
            _movementController  = context.Movement;
            _animationController = context.Animation;

            _rigidbody             = context.View.Rigidbody;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            _isInitialized = true;
        }

        public void FixedTick()
        {
            if (!_isEnabled || !_isInitialized)
                return;
            
            _movementController.FixedTick();
        }

        public void LateTick()
        {
            if (!_isEnabled || !_isInitialized)
                return;
            
            _animationController.UpdateMovementState(
                _rigidbody.velocity,
                false,
                _movementController.IsGrounded
            );
        }

        public void Enable()
            => _isEnabled = true;

        public void Disable()
            => _isEnabled = false;
    }
}
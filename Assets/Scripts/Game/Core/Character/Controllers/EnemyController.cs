using Game.Core.Animation;
using Game.Core.Movement;
using UnityEngine;
using Zenject;

namespace Game.Core.Character
{
    public class EnemyController : IEnemyController, ILateTickable, IFixedTickable
    {
        private readonly IMovementController  _movementController;
        private readonly IAnimationController _animationController;
        
        private Rigidbody _rigidbody;

        private bool _isEnabled = true;
        private bool _isInitialized;

        public EnemyController(
            IMovementController  movementController,
            IAnimationController animationController
        )
        {
            _movementController  = movementController;
            _animationController = animationController;
        }

        public void Initialize(ICharacterView view)
        {
            _movementController.Initialize(view);
            _animationController.Initialize(view);

            _rigidbody = view.Rigidbody;
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
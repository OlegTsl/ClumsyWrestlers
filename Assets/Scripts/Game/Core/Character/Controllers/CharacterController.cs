using System;
using Game.Common.Input;
using Game.Core.Animation;
using Game.Core.Movement;
using UnityEngine;
using Zenject;

namespace Game.Core.Character
{
    public class CharacterController : ICharacterController, ILateTickable, IDisposable
    {
        private readonly InputEventBus       _inputEvents;
        private readonly IMovementController  _movementController;
        private readonly IAnimationController _animationController;
        
        private Camera    _camera;
        private Rigidbody _rigidbody;

        private bool _isEnabled = true;
        private bool _isInitialized;

        public CharacterController(
            InputEventBus        inputEvents, 
            IMovementController  movementController,
            IAnimationController animationController
        )
        {
            _inputEvents         = inputEvents;
            _movementController  = movementController;
            _animationController = animationController;
            
            _inputEvents.Subscribe<MoveInput>(OnMove);
            _inputEvents.Subscribe<JumpAction>(OnJump);
        }

        public void Initialize(ICharacterView view)
        {
            _movementController.Initialize(view);
            _animationController.Initialize(view);

            _rigidbody     = view.Rigidbody;
            _isInitialized = true;
        }

        public void LateTick()
        {
            if (!_isInitialized)
                return;
            
            _animationController.UpdateMovementState(
                _rigidbody.velocity,
                _movementController.IsGrounded
            );
        }

        public void Enable()
            => _isEnabled = true;

        public void Disable()
            => _isEnabled = false;

        private void OnMove(MoveInput input)
        {
            if (!_isEnabled || !_isInitialized)
                return;
            
            Vector3 worldDirection = TransformInputToWorld(input.Direction);
            _movementController.SetMoveDirection(worldDirection);
        }

        private void OnJump(JumpAction action)
        {
            if (!_isEnabled || !_isInitialized)
                return;
            
            if (action.EventType == InputEventType.Pressed)
            {
                _movementController.Jump();
                _animationController.TriggerJump();
            }
        }
        
        private Vector3 TransformInputToWorld(Vector3 input)
        {
            if (_camera == null)
                _camera = Camera.main;
            
            Vector3 cameraForward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
            Vector3 cameraRight   = Vector3.ProjectOnPlane(_camera.transform.right, Vector3.up).normalized;
            
            return (cameraForward * input.z + cameraRight * input.x).normalized;
        }

        public void Dispose()
        {
            _inputEvents.Unsubscribe<MoveInput>(OnMove);
            _inputEvents.Unsubscribe<JumpAction>(OnJump);
        }
    }
}
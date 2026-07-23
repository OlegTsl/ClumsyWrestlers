using System;
using Game.Common.Input;
using Game.Core.Animation;
using Game.Core.Movement;
using UnityEngine;
using Zenject;

namespace Game.Core.Character
{
    public class CharacterController : ICharacterController, ILateTickable, IFixedTickable, IDisposable
    {
        private readonly InputEventBus       _inputEvents;
        private readonly IMovementController  _movementController;
        private readonly IAnimationController _animationController;
        private readonly ILookController      _lookController;
        
        private Rigidbody _rigidbody;

        private bool _isEnabled = true;
        private bool _hasMoveInput;
        private bool _isInitialized;

        public CharacterController(
            InputEventBus        inputEvents, 
            IMovementController  movementController,
            IAnimationController animationController,
            ILookController      lookController
        )
        {
            _inputEvents         = inputEvents;
            _movementController  = movementController;
            _animationController = animationController;
            _lookController      = lookController;
            
            _inputEvents.Subscribe<MoveInput>(OnMove);
            _inputEvents.Subscribe<JumpAction>(OnJump);
        }

        public void Initialize(ICharacterView view)
        {
            _movementController.Initialize(view);
            _animationController.Initialize(view);
            _lookController.Initialize(view);

            _rigidbody     = view.Rigidbody;
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
                _hasMoveInput,
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

            _hasMoveInput = input.Direction.magnitude > 0.01f;
            _movementController.SetMoveDirection(input.Direction);
        }

        private void OnJump(JumpAction action)
        {
            if (!_isEnabled || !_isInitialized)
                return;
            
            if (action.EventType == InputEventType.Pressed && _movementController.IsGrounded)
            {
                _movementController.Jump();
                _animationController.TriggerJump();
            }
        }

        public void Dispose()
        {
            _inputEvents.Unsubscribe<MoveInput>(OnMove);
            _inputEvents.Unsubscribe<JumpAction>(OnJump);
        }
    }
}
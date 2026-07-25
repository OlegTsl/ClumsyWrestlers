using System;
using Game.Common.Input;
using Game.Core.Animation;
using Game.Core.Combat;
using Game.Core.Movement;
using UnityEngine;
using Zenject;

namespace Game.Core.Character
{
    public class CharacterController : ICharacterController, ILateTickable, IFixedTickable, IDisposable
    {
        private readonly InputEventBus _inputEventsBus;

        private IMovementController  _movementController;
        private ICombatController    _combatController;
        private IAnimationController _animationController;        
        private Rigidbody         _rigidbody;

        private bool _isEnabled = true;
        private bool _hasMoveInput;
        private bool _isInitialized;

        public CharacterController(InputEventBus inputEventsBus)
        {
            _inputEventsBus = inputEventsBus;

            _inputEventsBus.Subscribe<MoveInput>(OnMove);
            _inputEventsBus.Subscribe<JumpAction>(OnJump);
        }

        public void Initialize(ICharacterContext context)
        {
            _movementController  = context.Movement;
            _combatController    = context.Combat;
            _animationController = context.Animation;
            _rigidbody           = context.View.Rigidbody;

            _isInitialized = true;
        }

        public void FixedTick()
        {
            if (!_isEnabled || !_isInitialized)
                return;

            _movementController.FixedTick();
            _combatController.FixedTick();
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
            _inputEventsBus.Unsubscribe<MoveInput>(OnMove);
            _inputEventsBus.Unsubscribe<JumpAction>(OnJump);
        }
    }
}
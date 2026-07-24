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
        private readonly InputEventBus        _inputEvents;
        private readonly IMovementController  _movementController;
        private readonly IAnimationController _animationController;
        private readonly ICombatController    _combatController;
        private readonly ILookController      _lookController;
        private readonly IDamageController    _damageController;
        private readonly IHitController       _hitController;
        
        private Rigidbody _rigidbody;

        private bool _isEnabled = true;
        private bool _hasMoveInput;
        private bool _isInitialized;

        public CharacterController(
            InputEventBus        inputEvents, 
            IMovementController  movementController,
            IAnimationController animationController,
            ICombatController    combatController,
            ILookController      lookController,
            IDamageController    damageController,
            IHitController       hitController
        )
        {
            _inputEvents         = inputEvents;
            _movementController  = movementController;
            _animationController = animationController;
            _combatController    = combatController;
            _lookController      = lookController;
            _damageController    = damageController;
            _hitController       = hitController;
            
            _inputEvents.Subscribe<MoveInput>(OnMove);
            _inputEvents.Subscribe<JumpAction>(OnJump);
        }

        public void Initialize(ICharacterView view)
        {
            _movementController.Initialize(view);
            _animationController.Initialize(view);
            _lookController.Initialize(view);
            _hitController.Initialize(view.AttackColliders);
            _damageController.Initialize(view.Transform, view.Data.AttackSettings,
                _hitController, _movementController);
            _combatController.Initialize(view, _animationController, _hitController);
            view.ColliderHandler.Initialize(_hitController);

            _rigidbody     = view.Rigidbody;
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
            _inputEvents.Unsubscribe<MoveInput>(OnMove);
            _inputEvents.Unsubscribe<JumpAction>(OnJump);
        }
    }
}
using System;
using Game.Common.Input;
using Game.Core.Movement;
using UnityEngine;

namespace Game.Core.Character
{
    public class CharacterController : ICharacterController, IDisposable
    {
        private readonly InputEventBus      _inputEvents;
        private readonly MovementController _movementController;
        
        private Camera _camera;

        private bool _isEnabled = true;
        private bool _isInitialized;

        public CharacterController(
            InputEventBus      inputEvents, 
            MovementController movementController
        )
        {
            _inputEvents        = inputEvents;
            _movementController = movementController;
            
            _inputEvents.Subscribe<MoveInput>(OnMove);
            _inputEvents.Subscribe<JumpAction>(OnJump);
        }

        public void Initialize(ICharacterView view)
        {
            _movementController.Initialize(view);
            _isInitialized = true;
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
                _movementController.Jump();
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
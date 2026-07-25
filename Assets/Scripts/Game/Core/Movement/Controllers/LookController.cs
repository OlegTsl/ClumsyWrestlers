using System;
using Game.Common.Input;
using UnityEngine;
using Zenject;

namespace Game.Core.Movement
{
    public class LookController : ILookController, IDisposable
    {
        private readonly InputEventsBus _inputEvents;
        
        private Transform _cameraTransform;
        private Transform _characterTransform;
        private Vector2 _lookDelta;

        private float _pitch;
        private float _yaw;
        
        public LookController(
            Transform      characterTransform,
            Transform      cameraTransform,
            InputEventsBus inputEvents)
        {
            _characterTransform = characterTransform;
            _cameraTransform    = cameraTransform;
            
            _yaw   = _characterTransform.eulerAngles.y;
            _pitch = _cameraTransform.localEulerAngles.x;

            _inputEvents = inputEvents;
            _inputEvents.Subscribe<LookInput>(OnLook);
        }
                
        private void OnLook(LookInput input)
            => _lookDelta += input.Delta;
        
        public void LateTick()
        {
            if (_cameraTransform == null)
                return;
            
            _yaw += _lookDelta.x;

            _pitch -= _lookDelta.y;
            _pitch  = Mathf.Clamp(_pitch, -89f, 89f);
            _lookDelta = Vector2.zero;
            
            _characterTransform.rotation = Quaternion.Euler(0f, _yaw, 0f);
            _cameraTransform.rotation    = Quaternion.Euler(_pitch, _yaw, 0f);
        }
        
        public void Dispose()
            => _inputEvents.Unsubscribe<LookInput>(OnLook);
    }
}
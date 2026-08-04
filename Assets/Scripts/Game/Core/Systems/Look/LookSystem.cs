using Game.Common.Input;
using UnityEngine;

namespace Game.Core.Systems
{
    public class LookSystem : ILookSystem
    {
        private readonly InputEventsBus _inputEventsBus;
        
        private Transform _cameraTransform;
        private Transform _characterTransform;
        private Vector2   _lookDelta;
        private float _pitch;
        private float _yaw;

        public LookSystem(
            InputEventsBus inputEventsBus,
            Transform      characterTransform,
            Transform      cameraTransform
        )
        {
            _inputEventsBus = inputEventsBus;
            _inputEventsBus.Subscribe<LookInput>(OnLook);

            _characterTransform = characterTransform;
            _cameraTransform    = cameraTransform;

            _yaw   = _characterTransform.eulerAngles.y;
            _pitch = _cameraTransform.localEulerAngles.x;

        }

        private void OnLook(LookInput input)
            => _lookDelta += input.Delta;

        public void LateTick()
        {
            if (_cameraTransform == null)
                return;

            _yaw      += _lookDelta.x;
            _pitch    -= _lookDelta.y;
            _pitch     = Mathf.Clamp(_pitch, -89f, 89f);
            _lookDelta = Vector2.zero;

            _characterTransform.rotation = Quaternion.Euler(0f, _yaw, 0f);
            _cameraTransform.rotation    = Quaternion.Euler(_pitch, _yaw, 0f);
        }

        public void Dispose()
        {
            _inputEventsBus.Unsubscribe<LookInput>(OnLook);
        }
    }
}
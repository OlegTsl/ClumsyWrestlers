using UnityEngine;

namespace Game.Core.Cameras
{
    public sealed class CameraAspectController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        [SerializeField, Min(0.01f)] private float _referenceOrthographicSize = 4f;
        [SerializeField, Min(1)]     private int   _referenceWidth = 16;
        [SerializeField, Min(1)]     private int   _referenceHeight = 9;

        private int _screenWidth;
        private int _screenHeight;

        private void OnEnable()
            => ApplyFraming();

        private void OnPreCull()
        {
            if (_screenWidth == Screen.width && _screenHeight == Screen.height)
                return;

            ApplyFraming();
        }

        private void ApplyFraming()
        {
            _screenWidth  = Mathf.Max(Screen.width, 1);
            _screenHeight = Mathf.Max(Screen.height, 1);

            float referenceAspect = (float)_referenceWidth / _referenceHeight;
            float currentAspect   = (float)_screenWidth / _screenHeight;
            
            _camera.orthographicSize = _referenceOrthographicSize * referenceAspect / currentAspect;
        }
    }
}

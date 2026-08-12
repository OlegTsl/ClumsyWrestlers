using UnityEngine;

namespace Game.Core.Systems
{
    public class LookSystem : ILookSystem
    {
        private readonly Transform _character;
        private readonly Transform _camera;
        private readonly Vector3   _offset = new Vector3(0f, 0f, -3f);

        public LookSystem(Transform character, Transform camera)
        {
            _character = character;
            _camera    = camera;
        }

        public void LateTick()
        {
            _camera.position = new Vector3(
                _character.position.x + _offset.x, 
                _camera.position.y, 
                _character.position.z + _offset.z
            );
        }
    }
}
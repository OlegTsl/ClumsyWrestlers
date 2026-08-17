using UnityEngine;

namespace Game.Core.Systems
{
    public sealed class LookSystem : ILookSystem
    {
        private static readonly Vector3 Offset = new(0f, 0f, -3f);

        private readonly Transform _character;
        private readonly Transform _camera;

        public LookSystem(Transform character, Transform camera)
        {
            _character = character;
            _camera = camera;
        }

        public void LateTick()
        {
            Vector3 characterPosition = _character.position;
            _camera.position = new Vector3(
                characterPosition.x + Offset.x,
                _camera.position.y,
                characterPosition.z + Offset.z);
        }

        public void Dispose()
        {
        }
    }
}

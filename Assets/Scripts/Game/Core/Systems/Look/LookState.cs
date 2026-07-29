using UnityEngine;

namespace Game.Core.Systems
{
    public sealed class LookState
    {
        public Transform CameraTransform;
        public Transform CharacterTransform;
        public Vector2   LookDelta;
        public float     Pitch;
        public float     Yaw;
    }
}

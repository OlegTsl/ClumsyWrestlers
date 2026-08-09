using UnityEngine;

namespace Game.Core.Systems
{
    public sealed class MovementState
    {
        public Vector3 MoveDirection;
        public Vector3 AirVelocity;
        public Vector3 HorizontalVelocity;
        public bool    JumpRequested;
        public float   VerticalVelocity;
        public float   AirTime;
        public bool    IsFalling;
        public bool    IsControllable = true;
    }
}

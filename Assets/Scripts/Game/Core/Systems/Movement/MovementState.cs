using UnityEngine;

namespace Game.Core.Systems
{
    public sealed class MovementState
    {
        public Vector3 MoveDirection { get; set; }
        public Vector3 AirVelocity { get; set; }
        public Vector3 HorizontalVelocity { get; set; }
        public bool JumpRequested { get; set; }
        public float VerticalVelocity { get; set; }
        public float AirTime { get; set; }
        public bool IsFalling { get; set; }
    }
}

using UnityEngine;

namespace Game.Core.Character
{
    public readonly struct CharacterPhysicsSnapshot
    {
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public Vector3 Velocity { get; }
        public Vector3 AttackOrigin { get; }
        public bool IsGrounded { get; }

        public CharacterPhysicsSnapshot(
            Vector3 position,
            Quaternion rotation,
            Vector3 velocity,
            Vector3 attackOrigin,
            bool isGrounded
        )
        {
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
            AttackOrigin = attackOrigin;
            IsGrounded = isGrounded;
        }
    }
}

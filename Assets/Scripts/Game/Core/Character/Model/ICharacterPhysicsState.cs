using UnityEngine;

namespace Game.Core.Character
{
    public interface ICharacterPhysicsState : ICharacterModel
    {
        Vector3 Velocity { get; }
        bool IsGrounded { get; }

        void SetVelocity(Vector3 velocity);
        void SynchronizePhysics(in CharacterPhysicsSnapshot snapshot);
    }
}

using Game.Core.Character;
using UnityEngine;

namespace Game.Core.Movement
{
    public interface IMovementController
    {
        void SetMoveDirection(Vector3 direction);
        void ApplyExternalForce(Vector3 force);
        void Jump();
        void FixedTick();

        bool IsGrounded { get; }
    }
}
using Game.Core.Character;
using UnityEngine;

namespace Game.Core.Movement
{
    public interface IMovementController
    {
        void Initialize(ICharacterView view);
        void SetMoveDirection(Vector3 direction);
        void Jump();
        void FixedTick();

        bool IsGrounded { get; }
    }
}
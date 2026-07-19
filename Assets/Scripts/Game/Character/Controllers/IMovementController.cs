using UnityEngine;

namespace Game.Character
{
    public interface IMovementController
    {
        void Initialize(ICharacterView view);
        void SetMoveDirection(Vector3 direction);
        void Jump();
        void FixedTick();
    }
}
using Game.Core.Character;
using UnityEngine;

namespace Game.Core.Animation
{
    public interface IAnimationController
    {
        void Initialize(ICharacterView view);
        void UpdateMovementState(Vector3 velocity, bool isMoving, bool isGrounded);
        void TriggerJump();
        void TriggerPunch();
    }
}
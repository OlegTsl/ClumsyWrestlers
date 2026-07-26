using UnityEngine;

namespace Game.Core.Animation
{
    public interface IAnimationController
    {
        void UpdateMovementState(Vector3 velocity, bool isMoving, bool isGrounded);
        void TriggerJump();
    }
}
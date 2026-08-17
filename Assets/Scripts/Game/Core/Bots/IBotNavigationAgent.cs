using UnityEngine;

namespace Game.Core.Bots
{
    public interface IBotNavigationAgent
    {
        void Activate(Vector3 position, IBotBehaviorSettings settings);
        bool TryGetMovement(
            Vector3 position,
            Vector3 destination,
            bool isGrounded,
            out Vector3 direction,
            out bool shouldJump);
        bool CanStandAt(Vector3 position, float edgeClearance);
        bool TryGetDistanceToDropEdge(Vector3 position, out float distance);
        void Stop();
        void Deactivate();
    }
}

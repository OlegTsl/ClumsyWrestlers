using UnityEngine;

namespace Game.Core.Extensions
{
    public static class CollisionsExtension
    {
        public static int OverlapCapsule(Vector3 start, Vector3 end, float radius, Collider[] results, int layer)
        {
            return Physics.OverlapCapsuleNonAlloc(start, end,
                radius, results, layer, QueryTriggerInteraction.Collide);
        }

        public static int OverlapSphere(Vector3 position, float radius, Collider[] results, int layer)
        {
            return Physics.OverlapSphereNonAlloc(position,
                radius, results, layer, QueryTriggerInteraction.Ignore);
        }
    }
}

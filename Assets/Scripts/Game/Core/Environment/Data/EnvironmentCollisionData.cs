using UnityEngine;

namespace Game.Core.Environment
{
    public sealed class EnvironmentCollisionData
    {
        public Collider OtherCollider { get; }
        public Vector3 ImpactVelocity { get; }
        public float ImpactSpeed => ImpactVelocity.magnitude;

        public EnvironmentCollisionData(Collider otherCollider, Vector3 impactVelocity)
        {
            OtherCollider  = otherCollider;
            ImpactVelocity = impactVelocity;
        }
    }
}

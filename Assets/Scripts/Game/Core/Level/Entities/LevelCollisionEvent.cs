using Game.Core.Entities;
using UnityEngine;

namespace Game.Core.Level.Entities
{
    public readonly struct LevelCollisionEvent
    {
        public EntityId SourceId { get; }
        public Collider OtherCollider { get; }
        public Vector3 ImpactVelocity { get; }
        public float ImpactSpeed { get; }

        public LevelCollisionEvent(
            EntityId sourceId,
            Collider otherCollider,
            Vector3 impactVelocity
        )
        {
            SourceId = sourceId;
            OtherCollider = otherCollider;
            ImpactVelocity = impactVelocity;
            ImpactSpeed = impactVelocity.magnitude;
        }
    }
}

using Game.Core.Entities;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.GameEvents
{
    public readonly struct OnLevelEntityImpulseRequestedEvent
    {
        public EntityId EntityId { get; }
        public Vector3 Impulse { get; }

        public OnLevelEntityImpulseRequestedEvent(EntityId entityId, Vector3 impulse)
        {
            EntityId = entityId;
            Impulse = impulse;
        }
    }

    public readonly struct OnLevelEntityDampingRequestedEvent
    {
        public EntityId EntityId { get; }
        public Vector3 ImpactVelocity { get; }

        public OnLevelEntityDampingRequestedEvent(
            EntityId entityId,
            Vector3 impactVelocity
        )
        {
            EntityId = entityId;
            ImpactVelocity = impactVelocity;
        }
    }
}

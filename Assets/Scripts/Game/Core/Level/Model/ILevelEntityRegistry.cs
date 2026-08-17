using System.Collections.Generic;
using Game.Core.Entities;
using Game.Core.Level.Entities;
using UnityEngine;

namespace Game.Core.Level
{
    public interface ILevelEntityRegistry
    {
        IReadOnlyList<ILevelEntityView> LevelEntities { get; }
        bool TryGetEntityId(Collider hitbox, out EntityId entityId);
        bool TryGetEntity(EntityId entityId, out ILevelEntityView entity);
        bool TryGetCapability<TCapability>(
            EntityId entityId,
            out TCapability capability)
            where TCapability : class;
    }
}

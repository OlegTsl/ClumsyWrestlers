using System;
using System.Collections.Generic;
using Game.Core.Entities;
using Game.Core.Level;
using Game.Core.Level.Entities;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Tests.Core
{
    public sealed class EmptyLevelRegistry :
        ILevelEntityRegistry,
        ILevelImpactSettingsRegistry
    {
        public IReadOnlyList<ILevelEntityView> LevelEntities { get; } =
            Array.Empty<ILevelEntityView>();

        public bool TryGetEntityId(Collider hitbox, out EntityId entityId)
        {
            entityId = EntityId.Invalid;
            return false;
        }

        public bool TryGetEntity(
            EntityId entityId,
            out ILevelEntityView entity)
        {
            entity = null;
            return false;
        }

        public bool TryGetCapability<TCapability>(
            EntityId entityId,
            out TCapability capability)
            where TCapability : class
        {
            capability = null;
            return false;
        }

        public bool TryGetImpactSettings(
            EntityId entityId,
            out LevelEntityImpactSettings settings)
        {
            settings = default;
            return false;
        }
    }
}

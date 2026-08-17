using System;
using System.Collections.Generic;
using Game.Core.Entities;
using Game.Core.Level.Entities;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Level
{
    public sealed class LevelModel :
        ILevelModel,
        ILevelEntityRegistry,
        ILevelFixedTickSource,
        ILevelSpawnPointProvider,
        ILevelImpactSettingsRegistry,
        ILevelCollisionBuffer,
        ILevelCollisionSink
    {
        private const int CollisionBufferCapacity = 256;

        private readonly ILevelView _view;
        private readonly Dictionary<EntityId, ILevelEntityView> _entities;
        private readonly Dictionary<Collider, EntityId> _colliderIds;
        private readonly Dictionary<EntityId, LevelEntityImpactSettings> _impactSettings;
        private readonly List<ILevelEntityFixedTickView> _fixedTickEntities;
        private readonly LevelCollisionEvent[] _collisionBuffer =
            new LevelCollisionEvent[CollisionBufferCapacity];

        private int _collisionReadIndex;
        private int _collisionWriteIndex;
        private int _collisionCount;

        public IReadOnlyList<ILevelEntityView> LevelEntities => _view.LevelEntities;
        public IReadOnlyList<ILevelEntityFixedTickView> FixedTickEntities
            => _fixedTickEntities;

        public LevelModel(ILevelView view)
        {
            _view = view;
            int entityCount = view.LevelEntities.Count;
            _entities = new Dictionary<EntityId, ILevelEntityView>(entityCount);
            _colliderIds = new Dictionary<Collider, EntityId>(entityCount * 2);
            _impactSettings = new Dictionary<EntityId, LevelEntityImpactSettings>(entityCount);
            _fixedTickEntities = new List<ILevelEntityFixedTickView>(entityCount);

            for (int i = 0; i < entityCount; i++)
            {
                Register(view.LevelEntities[i]);
            }
        }

        public LevelSpawnPoint GetCharacterSpawnPoint(bool isPlayer)
            => _view.GetCharacterSpawnPoint(isPlayer);

        public bool TryGetEntityId(Collider hitbox, out EntityId entityId)
            => _colliderIds.TryGetValue(hitbox, out entityId);

        public bool TryGetEntity(EntityId entityId, out ILevelEntityView entity)
            => _entities.TryGetValue(entityId, out entity);

        public bool TryGetCapability<TCapability>(
            EntityId entityId,
            out TCapability capability
        ) where TCapability : class
        {
            if (_entities.TryGetValue(entityId, out ILevelEntityView entity) &&
                entity is TCapability typedCapability)
            {
                capability = typedCapability;
                return true;
            }

            capability = null;
            return false;
        }

        public bool TryGetImpactSettings(
            EntityId entityId,
            out LevelEntityImpactSettings settings)
            => _impactSettings.TryGetValue(entityId, out settings);

        public bool TryEnqueue(in LevelCollisionEvent collisionEvent)
        {
            if (_collisionCount >= _collisionBuffer.Length)
            {
                return false;
            }

            _collisionBuffer[_collisionWriteIndex] = collisionEvent;
            _collisionWriteIndex = (_collisionWriteIndex + 1) % _collisionBuffer.Length;
            _collisionCount++;
            return true;
        }

        public bool TryDequeueCollision(out LevelCollisionEvent collisionEvent)
        {
            if (_collisionCount == 0)
            {
                collisionEvent = default;
                return false;
            }

            collisionEvent = _collisionBuffer[_collisionReadIndex];
            _collisionBuffer[_collisionReadIndex] = default;
            _collisionReadIndex = (_collisionReadIndex + 1) % _collisionBuffer.Length;
            _collisionCount--;
            return true;
        }

        private void Register(ILevelEntityView entity)
        {
            if (!entity.EntityId.IsValid)
            {
                throw new InvalidOperationException(
                    $"Level entity '{entity}' has an invalid authored id.");
            }

            if (!_entities.TryAdd(entity.EntityId, entity))
            {
                throw new InvalidOperationException(
                    $"Level contains duplicate entity id {entity.EntityId}.");
            }

            IReadOnlyList<Collider> hitboxes = entity.Hitboxes;
            for (int i = 0; i < hitboxes.Count; i++)
            {
                Collider hitbox = hitboxes[i];
                if (!_colliderIds.TryAdd(hitbox, entity.EntityId))
                {
                    throw new InvalidOperationException(
                        $"Level entities share collider '{hitbox.name}'.");
                }
            }

            if (entity is ILevelEntityImpactSettingsProvider settingsProvider)
            {
                _impactSettings.Add(entity.EntityId, settingsProvider.ImpactSettings);
            }

            if (entity is ILevelCollisionEmitter collisionEmitter)
            {
                collisionEmitter.BindCollisionSink(this);
            }

            if (entity is ILevelEntityFixedTickView fixedTickEntity)
            {
                _fixedTickEntities.Add(fixedTickEntity);
            }
        }

        public void Dispose()
        {
            IReadOnlyList<ILevelEntityView> entities = _view.LevelEntities;
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] is ILevelCollisionEmitter collisionEmitter)
                {
                    collisionEmitter.BindCollisionSink(null);
                }
            }

            _collisionReadIndex = 0;
            _collisionWriteIndex = 0;
            _collisionCount = 0;
            _entities.Clear();
            _colliderIds.Clear();
            _impactSettings.Clear();
            _fixedTickEntities.Clear();
        }
    }
}

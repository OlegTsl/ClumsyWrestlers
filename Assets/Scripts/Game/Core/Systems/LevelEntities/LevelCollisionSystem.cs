using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.Entities;
using Game.Core.GameEvents;
using Game.Core.Level;
using Game.Core.Level.Entities;
using UnityEngine;
using Zenject;

namespace Game.Core.Systems
{
    public sealed class LevelCollisionSystem : IDisposable, IFixedTickable
    {
        private const float MinimumImpactSpeed = 0.3f;

        private readonly IGameEventsBus _events;
        private readonly ILevelCollisionBuffer _collisions;
        private readonly ILevelEntityRegistry _levelEntities;
        private readonly ICharacterContext _characters;
        private readonly ICharacterViewContext _characterViews;
        private readonly HashSet<CollisionPair> _processedCollisions = new(32);

        public LevelCollisionSystem(
            IGameEventsBus events,
            ILevelCollisionBuffer collisions,
            ILevelEntityRegistry levelEntities,
            ICharacterContext characters,
            ICharacterViewContext characterViews
        )
        {
            _events = events;
            _collisions = collisions;
            _levelEntities = levelEntities;
            _characters = characters;
            _characterViews = characterViews;
        }

        public void FixedTick()
        {
            _processedCollisions.Clear();
            while (_collisions.TryDequeueCollision(out LevelCollisionEvent collision))
            {
                PublishCharacterHit(collision);
            }
        }

        private void PublishCharacterHit(in LevelCollisionEvent collision)
        {
            if (collision.ImpactSpeed < MinimumImpactSpeed ||
                !_characterViews.TryGetCharacterId(
                    collision.OtherCollider,
                    out EntityId targetId))
            {
                return;
            }

            ICharacterModel target = _characters.GetModel(targetId);
            if (target == null ||
                !_processedCollisions.Add(new CollisionPair(
                    collision.SourceId,
                    targetId)) ||
                !_levelEntities.TryGetEntity(
                    collision.SourceId,
                    out ILevelEntityView source))
            {
                return;
            }

            ICharacterCombatRuntimeState combat =
                target.GetState<ICharacterCombatRuntimeState>();
            if (!combat.Enabled)
            {
                return;
            }

            Vector3 direction = combat.Position - source.Position;
            _events.Publish(new OnHitDetectedEvent(new HitData(
                HitObjectType.LevelEntity,
                collision.SourceId,
                HitObjectType.Character,
                targetId,
                AttackType.Simple,
                direction,
                collision.ImpactVelocity)));
        }

        public void Dispose()
            => _processedCollisions.Clear();

        private readonly struct CollisionPair : IEquatable<CollisionPair>
        {
            private readonly EntityId _sourceId;
            private readonly EntityId _targetId;

            public CollisionPair(EntityId sourceId, EntityId targetId)
            {
                _sourceId = sourceId;
                _targetId = targetId;
            }

            public bool Equals(CollisionPair other)
                => _sourceId == other._sourceId && _targetId == other._targetId;

            public override bool Equals(object obj)
                => obj is CollisionPair other && Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_sourceId.GetHashCode() * 397) ^
                           _targetId.GetHashCode();
                }
            }
        }
    }
}

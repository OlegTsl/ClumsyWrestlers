using System.Collections.Generic;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Level.Entities
{
    public abstract class LevelEntityView : MonoBehaviour, ILevelEntityView
    {
        [SerializeField, Min(1)] private int _authoredEntityId = 1;

        public EntityId EntityId => new((uint)_authoredEntityId);
        public abstract IReadOnlyList<Collider> Hitboxes { get; }
        public abstract Vector3 Position { get; }
    }
}

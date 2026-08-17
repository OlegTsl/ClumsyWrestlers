using Game.Core.Entities;
using System.Collections.Generic;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Level.Entities
{
    public interface ILevelEntityView
    {
        EntityId EntityId { get; }
        IReadOnlyList<Collider> Hitboxes { get; }
        Vector3 Position { get; }
    }
}

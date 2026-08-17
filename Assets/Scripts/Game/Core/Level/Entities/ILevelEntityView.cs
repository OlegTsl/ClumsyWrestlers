using Game.Core.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Level.Entities
{
    public interface ILevelEntityView
    {
        EntityId EntityId { get; }
        IReadOnlyList<Collider> Hitboxes { get; }
        Vector3 Position { get; }
    }
}

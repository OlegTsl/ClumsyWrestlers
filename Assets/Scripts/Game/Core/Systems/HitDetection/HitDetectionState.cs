using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Systems
{
    public sealed class HitDetectionState
    {
        public readonly List<Collider> Targets = new();
        public bool Enabled;
    }
}

using UnityEngine;

namespace Game.Core.Systems
{
    internal sealed class HitReactionState
    {
        public bool Active { get; set; }
        public float Elapsed { get; set; }
        public float Duration { get; set; }
        public Quaternion StartRotation { get; set; } = Quaternion.identity;
        public Quaternion TargetRotation { get; set; } = Quaternion.identity;
        public Quaternion CurrentRotation { get; set; } = Quaternion.identity;
    }
}

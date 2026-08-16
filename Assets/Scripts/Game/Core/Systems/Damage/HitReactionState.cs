using UnityEngine;

namespace Game.Core.Systems
{
    internal sealed class HitReactionState
    {
        public bool  Active;
        public float Elapsed;
        public float Duration;
        public Quaternion StartRotation   = Quaternion.identity;
        public Quaternion TargetRotation  = Quaternion.identity;
        public Quaternion CurrentRotation = Quaternion.identity;
    }
}

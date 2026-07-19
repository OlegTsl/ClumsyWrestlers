using UnityEngine;

namespace Game.Common.Input
{
    public readonly struct MoveInput
    {
        public Vector3 Direction { get; }
        public bool IsActive => 
            Direction != Vector3.zero;

        public MoveInput(Vector3 direction)
            => Direction = direction.normalized;

        public static MoveInput None
            => new(Vector3.zero);
    }
}
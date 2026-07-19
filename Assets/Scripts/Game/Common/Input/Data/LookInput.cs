using UnityEngine;

namespace Game.Common.Input
{
    public readonly struct LookInput
    {
        public Vector2 Delta { get; }

        public bool IsActive
            => Delta != Vector2.zero;

        public LookInput(Vector2 delta)
            => Delta = delta;

        public static LookInput None
            => new(Vector2.zero);
    }
}
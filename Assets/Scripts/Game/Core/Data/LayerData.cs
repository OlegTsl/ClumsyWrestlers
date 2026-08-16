using UnityEngine;

namespace Game.Core.Data
{
    public static class LayerData
    {
        public static readonly int Ground = LayerMask.NameToLayer("Ground");
        public static readonly int Hitbox = LayerMask.NameToLayer("Hitbox");
        public static readonly int Environment = LayerMask.NameToLayer("Environment");
    }
}

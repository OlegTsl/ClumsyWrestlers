using UnityEngine;

namespace Game.Core.Systems
{
    public interface IAimSystem
    {
        void Rotate(Vector2 lookDelta);
        void Update();
    }
}

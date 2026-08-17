using Game.Core.Level.Entities;

namespace Game.Core.Level
{
    public interface ILevelCollisionBuffer
    {
        bool TryDequeueCollision(out LevelCollisionEvent collisionEvent);
    }
}

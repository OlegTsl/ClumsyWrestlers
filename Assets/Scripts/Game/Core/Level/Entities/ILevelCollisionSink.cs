namespace Game.Core.Level.Entities
{
    public interface ILevelCollisionSink
    {
        bool TryEnqueue(in LevelCollisionEvent collisionEvent);
    }
}

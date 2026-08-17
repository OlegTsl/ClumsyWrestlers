namespace Game.Core.Level.Entities
{
    public interface ILevelCollisionEmitter
    {
        void BindCollisionSink(ILevelCollisionSink collisionSink);
    }
}

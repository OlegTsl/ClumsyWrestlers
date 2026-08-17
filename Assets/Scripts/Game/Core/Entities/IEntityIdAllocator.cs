namespace Game.Core.Entities
{
    public interface IEntityIdAllocator
    {
        EntityId Allocate();
    }
}

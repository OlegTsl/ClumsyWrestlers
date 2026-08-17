using System;

namespace Game.Core.Entities
{
    public sealed class EntityIdAllocator : IEntityIdAllocator
    {
        private uint _nextId = EntityId.DynamicRangeStart;

        public EntityId Allocate()
        {
            if (_nextId == 0u)
            {
                throw new InvalidOperationException("The runtime entity id range is exhausted.");
            }

            return new EntityId(_nextId++);
        }
    }
}

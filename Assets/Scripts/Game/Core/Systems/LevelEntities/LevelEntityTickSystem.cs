using System;
using Game.Core.Level;
using Game.Core.Level.Entities;
using Zenject;

namespace Game.Core.Systems
{
    public sealed class LevelEntityTickSystem : IDisposable, IFixedTickable
    {
        private readonly ILevelFixedTickSource _tickSource;

        public LevelEntityTickSystem(ILevelFixedTickSource tickSource)
            => _tickSource = tickSource;

        public void FixedTick()
        {
            var entities = _tickSource.FixedTickEntities;
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].FixedTick();
            }
        }

        public void Dispose()
        {
        }
    }
}

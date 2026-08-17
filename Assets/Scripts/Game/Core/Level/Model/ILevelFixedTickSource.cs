using System.Collections.Generic;
using Game.Core.Level.Entities;

namespace Game.Core.Level
{
    public interface ILevelFixedTickSource
    {
        IReadOnlyList<ILevelEntityFixedTickView> FixedTickEntities { get; }
    }
}

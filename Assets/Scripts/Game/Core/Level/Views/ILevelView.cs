using System.Collections.Generic;
using Game.Common.Views;
using Game.Core.Level.Entities;

namespace Game.Core.Level
{
    public interface ILevelView : IView
    {
        IReadOnlyList<ILevelEntityView> LevelEntities { get; }
        LevelSpawnPoint GetCharacterSpawnPoint(bool isPlayer);
    }
}

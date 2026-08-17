using System.Collections.Generic;
using Game.Common.Views;
using Game.Core.Level.Entities;
using UnityEngine;

namespace Game.Core.Level
{
    public interface ILevelView : IView
    {
        IReadOnlyList<ILevelEntityView> LevelEntities { get; }
        Vector3 SafePosition { get; }
        float EliminationHeight { get; }
        LevelSpawnPoint GetCharacterSpawnPoint(
            bool isPlayerTeam,
            int spawnIndex);
    }
}

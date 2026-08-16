using System.Collections.Generic;
using Game.Common.Views;
using Game.Core.Environment;
using UnityEngine;

namespace Game.Core.Level
{
    public interface ILevelView : IView
    {
        IReadOnlyList<IInteractableChestView> InteractableChests { get; }

        Transform GetCharacterSpawnPosition(bool isPlayer);
    }
}

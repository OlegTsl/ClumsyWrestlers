using System;
using System.Collections.Generic;
using Game.Core.Environment;
using UnityEngine;

namespace Game.Core.Level
{
    public interface ILevelModel
    {
        IReadOnlyList<IInteractableChestView> InteractableChests { get; }

        Transform GetCharacterSpawnPosition(bool isPlayer);
        IInteractableChestView GetInteractableChest(Guid chestID);
        IInteractableChestView GetInteractableChest(Collider hitbox);
    }
}

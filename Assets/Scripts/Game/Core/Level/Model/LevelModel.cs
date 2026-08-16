using System;
using System.Collections.Generic;
using Game.Core.Environment;
using UnityEngine;

namespace Game.Core.Level
{
    public sealed class LevelModel : ILevelModel
    {
        private readonly ILevelView _view;

        public IReadOnlyList<IInteractableChestView> InteractableChests
            => _view.InteractableChests;

        public LevelModel(ILevelView view)
            => _view = view;

        public Transform GetCharacterSpawnPosition(bool isPlayer)
            => _view.GetCharacterSpawnPosition(isPlayer);

        public IInteractableChestView GetInteractableChest(Guid chestID)
        {
            IReadOnlyList<IInteractableChestView> chests = _view.InteractableChests;
            for (int i = 0; i < chests.Count; i++)
            {
                if (chests[i].ChestID == chestID)
                {
                    return chests[i];
                }
            }

            return null;
        }

        public IInteractableChestView GetInteractableChest(Collider hitbox)
        {
            IReadOnlyList<IInteractableChestView> chests = _view.InteractableChests;
            for (int i = 0; i < chests.Count; i++)
            {
                if (chests[i].Hitbox == hitbox)
                {
                    return chests[i];
                }
            }

            return null;
        }
    }
}

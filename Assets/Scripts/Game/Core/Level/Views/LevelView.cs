using System;
using System.Collections.Generic;
using Game.Core.Environment;
using UnityEngine;

namespace Game.Core.Level
{
    public class LevelView : MonoBehaviour, ILevelView
    {
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private Transform _enemySpawnPoint;
        [SerializeField] private InteractableChestView[] _interactableChests;

        public IReadOnlyList<IInteractableChestView> InteractableChests => _interactableChests;

        public Transform GetCharacterSpawnPosition(bool isPlayer)
            => isPlayer ? _playerSpawnPoint : _enemySpawnPoint;

        public void Hide() => gameObject.SetActive(false);
        public void Show() => gameObject.SetActive(true);

        public Action DisposeAction { get; set; }
    }
}

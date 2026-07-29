using System;
using UnityEngine;

namespace Game.Core.Level
{
    public class LevelView : MonoBehaviour, ILevelView
    {
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private Transform _enemySpawnPoint;
        
        public Transform GetCharacterSpawnPosition(bool isPlayer)
            => isPlayer ? _playerSpawnPoint : _enemySpawnPoint;

        public void Hide() => gameObject.SetActive(false);
        public void Show() => gameObject.SetActive(true);

        public Action DisposeAction { get; set; }
    }
}
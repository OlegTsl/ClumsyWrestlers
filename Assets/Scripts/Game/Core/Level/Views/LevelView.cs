using System;
using UnityEngine;

namespace Game.Core.Level
{
    public class LevelView : MonoBehaviour, ILevelView
    {
        [SerializeField] private Transform _playerSpawnPoint;
        
        public Transform PlayerSpawnPoint => _playerSpawnPoint;

        public void Hide() => gameObject.SetActive(false);
        public void Show() => gameObject.SetActive(true);

        public Action DisposeAction { get; set; }
    }
}
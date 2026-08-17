using System.Collections.Generic;
using Game.Core.Level.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Core.Level
{
    public sealed class LevelView : MonoBehaviour, ILevelView
    {
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private Transform _enemySpawnPoint;
        [FormerlySerializedAs("_interactableChests")]
        [SerializeField] private LevelEntityView[] _levelEntities;

        public IReadOnlyList<ILevelEntityView> LevelEntities => _levelEntities;

        public LevelSpawnPoint GetCharacterSpawnPoint(bool isPlayer)
        {
            Transform spawnPoint = isPlayer ? _playerSpawnPoint : _enemySpawnPoint;
            return new LevelSpawnPoint(spawnPoint.position, spawnPoint.rotation);
        }

        public void Hide()
            => gameObject.SetActive(false);

        public void Show()
            => gameObject.SetActive(true);
    }
}

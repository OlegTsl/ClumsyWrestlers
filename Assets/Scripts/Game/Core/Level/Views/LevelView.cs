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
        [SerializeField, Min(0.1f)] private float _spawnSpacing = 1.5f;
        [SerializeField] private float _eliminationHeight = -5f;

        public IReadOnlyList<ILevelEntityView> LevelEntities => _levelEntities;
        public Vector3 SafePosition => transform.position;
        public float EliminationHeight => _eliminationHeight;

        public LevelSpawnPoint GetCharacterSpawnPoint(
            bool isPlayerTeam,
            int spawnIndex)
        {
            Transform spawnPoint = isPlayerTeam
                ? _playerSpawnPoint
                : _enemySpawnPoint;
            int pairIndex = (spawnIndex + 1) / 2;
            float side = spawnIndex % 2 == 0 ? -1f : 1f;
            Vector3 offset = pairIndex == 0
                ? Vector3.zero
                : spawnPoint.right * (pairIndex * side * _spawnSpacing);
            return new LevelSpawnPoint(
                spawnPoint.position + offset,
                spawnPoint.rotation);
        }

        public void Hide()
            => gameObject.SetActive(false);

        public void Show()
            => gameObject.SetActive(true);
    }
}

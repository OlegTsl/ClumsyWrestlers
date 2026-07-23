using Cysharp.Threading.Tasks;
using Game.Common.AssetsManager;
using Game.Core.Character;
using UnityEngine;

namespace Game.Core.Level
{
    public class LevelController : ILevelController
    {
        private readonly IAssetManager        _assetManager;
        private readonly ICharacterFactory    _characterFactory;
        private readonly ICharacterController _characterController;
        private readonly IEnemyController     _enemyController;

        private ILevelView     _level;
        private ICharacterView _player;
        private ICharacterView _enemy;

        private string _levelName;
        private string _playerName;
        private string _enemyName;

        public LevelController(
            IAssetManager        assetManager,
            ICharacterFactory    characterFactory,
            ICharacterController characterController,
            IEnemyController     enemyController
        )
        {
            _assetManager        = assetManager;
            _characterFactory    = characterFactory;
            _characterController = characterController;
            _enemyController     = enemyController;
        }

        public async UniTask LoadLevel(string address)
        {
            UnloadLevel();

            _level = await _assetManager.LoadView<ILevelView>(address);
            if (_level == null)
            {
                Debug.LogError($"Failed to load level: {address}");
                return;
            }

            _levelName = address;
            Debug.Log($"Level loaded: {address}");

            return;
        }

        public void UnloadLevel()
        {
            if (_level != null)
            {
                _assetManager.UnloadAsset(_levelName);
                Debug.LogError($"Level unloaded: {_levelName}");

                _levelName = "";
                _level     = null;
            }
            
            if (_player != null)
            {
                _assetManager.UnloadAsset(_playerName);
                Debug.LogError($"Player unloaded: {_playerName}");

                _playerName = "";
                _player     = null;

                _characterController.Disable();
            }

            if (_enemy != null)
            {
                _assetManager.UnloadAsset(_enemyName);
                Debug.LogError($"Enemy unloaded: {_enemyName}");

                _enemyName = "";
                _enemy     = null;

                _enemyController.Disable();
            }
        }

        public bool IsLevelLoaded()
            => _level != null;

        public async UniTask SpawnPlayer(string name)
        {
            if (_level == null)
            {
                Debug.LogError($"Level not loaded!");
                return;
            }

            var spawnPoint = _level.PlayerSpawnPoint;
            _player = await _characterFactory.Create(
                name, spawnPoint.position, spawnPoint.rotation);

            if (_player == null)
            {
                Debug.LogError($"Failed to load character: {name}");
                return;
            }
            
            _characterController.Initialize(_player);
            _characterController.Enable();
            
            _playerName = name;

            _player.SetAsPlayer(true);
            _player.Show();

            Debug.Log($"Player loaded: {name}");
        }

        public async UniTask SpawnEnemy(string name)
        {
            if (_level == null)
            {
                Debug.LogError($"Level not loaded!");
                return;
            }

            var spawnPoint = _level.EnemySpawnPoint;
            _enemy = await _characterFactory.Create(
                name, spawnPoint.position, spawnPoint.rotation);

            if (_enemy == null)
            {
                Debug.LogError($"Failed to load enemy: {name}");
                return;
            }

            _enemyController.Initialize(_enemy);
            _enemyController.Enable();

            _enemyName = name;

            _enemy.SetAsPlayer(false);
            _enemy.Show();

            Debug.Log($"Enemy loaded: {name}");
        }
    }
}
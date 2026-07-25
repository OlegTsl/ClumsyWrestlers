using Cysharp.Threading.Tasks;
using Game.Common.AssetsManager;
using Game.Core.Character;
using UnityEngine;

namespace Game.Core.Level
{
    public class LevelController : ILevelController
    {
        private readonly IAssetManager            _assetManager;
        private readonly ICharacterController     _characterController;
        private readonly IEnemyController         _enemyController;
        private readonly ICharactersRegistry      _charactersRegistry;
        private readonly ICharacterContextBuilder _characterContextBuilder;

        private ILevelView        _level;
        private ICharacterContext _playerContext;
        private ICharacterContext _enemyContext;

        private string _levelName;
        private string _playerName;
        private string _enemyName;

        public LevelController(
            IAssetManager            assetManager,
            ICharacterController     characterController,
            IEnemyController         enemyController,
            ICharactersRegistry      charactersRegistry,
            ICharacterContextBuilder characterContextBuilder
        )
        {
            _assetManager            = assetManager;
            _characterController     = characterController;
            _enemyController         = enemyController;
            _charactersRegistry      = charactersRegistry;
            _characterContextBuilder = characterContextBuilder;
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
            
            if (_playerContext != null)
            {
                _assetManager.UnloadAsset(_playerName);
                _charactersRegistry.Unregister(_playerContext);
                
                Debug.LogError($"Player unloaded: {_playerName}");

                _playerName    = "";
                _playerContext = null;

                _characterController.Disable();
            }

            if (_enemyContext != null)
            {
                _assetManager.UnloadAsset(_enemyName);
                _charactersRegistry.Unregister(_enemyContext);

                Debug.LogError($"Enemy unloaded: {_enemyName}");

                _enemyName    = "";
                _enemyContext = null;

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
            _playerContext = await _characterContextBuilder.BuildPlayerContext(
                name, spawnPoint.position, spawnPoint.rotation);
            
            if (_playerContext == null)
            {
                Debug.LogError($"Failed to load character: {name}");
                return;
            }

            _charactersRegistry.Register(_playerContext);
            
            _characterController.Initialize(_playerContext);
            _characterController.Enable();
            
            _playerName = name;

            _playerContext.View.SetAsPlayer(true);
            _playerContext.View.Show();

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
            _enemyContext = await _characterContextBuilder.BuildPlayerContext(
                name, spawnPoint.position, spawnPoint.rotation);

            if (_enemyContext == null)
            {
                Debug.LogError($"Failed to load enemy: {name}");
                return;
            }

            _charactersRegistry.Register(_enemyContext);

            _enemyController.Initialize(_enemyContext);
            _enemyController.Enable();

            _enemyName = name;

            _enemyContext.View.SetAsPlayer(false);
            _enemyContext.View.Show();

            Debug.Log($"Enemy loaded: {name}");
        }
    }
}
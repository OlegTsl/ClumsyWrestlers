using Cysharp.Threading.Tasks;
using Game.Common.AssetsManager;
using Game.Common.Input;
using Game.Core.Character;
using UnityEngine;

namespace Game.Core.Level
{
    public class LevelController : ILevelController
    {
        private readonly IAssetManager            _assetManager;
        private readonly ICharactersController    _charactersController;
        private readonly ICharacterContextBuilder _characterContextBuilder;

        private ILevelView _level;
        private string     _levelName;

        public LevelController(
            IAssetManager            assetManager,
            ICharactersController    charactersController,
            ICharacterContextBuilder characterContextBuilder
        )
        {
            _assetManager            = assetManager;
            _charactersController    = charactersController;
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

            _charactersController.RemoveAllCharacters();
        }

        public bool IsLevelLoaded()
            => _level != null;


        public async UniTask SpawnCharacter(string name, bool isPlayer)
        {
            if (_level == null)
            {
                Debug.LogError($"Level not loaded!");
                return;
            }

            Transform      spawnPoint;
            IInputSource[] inputSources;

            if (isPlayer)
            {
                spawnPoint   = _level.PlayerSpawnPoint;
                inputSources = new IInputSource[]
                {
                    new KeyboardSource(priority: 0),
                    new MouseSource   (priority: 1)
                };
            }
            else
            {
                spawnPoint   = _level.EnemySpawnPoint;
                inputSources = new IInputSource[] { };
            }

            var character = await _characterContextBuilder.BuildCharacterContext(
                name, isPlayer, spawnPoint.position, spawnPoint.rotation, inputSources);

            if (character == null)
            {
                Debug.LogError($"Failed to load character: {name}");
                return;
            }

            _charactersController.AddCharacter(character);
            character.View.Show();

            Debug.Log($"Player loaded: {name}");
        }
    }
}
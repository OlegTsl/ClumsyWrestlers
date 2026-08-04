using Cysharp.Threading.Tasks;
using Game.Common.AssetsManager;
using Game.Core.Character;
using UnityEngine;

namespace Game.Core.Level
{
    public class LevelController : ILevelController
    {
        private readonly IAssetManager _assetManager;

        private ILevelView _level;
        private string     _levelName;

        public LevelController(
            IAssetManager assetManager
        )
        {
            _assetManager = assetManager;
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
        }

        public bool IsLevelLoaded()
            => _level != null;


        public void SpawnCharacter(ICharacterModel model, bool isPlayer)
        {
            if (!IsLevelLoaded())
            {
                Debug.LogError($"Level not loaded!");
                return;
            }

            var spawnPoint = _level.GetCharacterSpawnPosition(isPlayer);
            model.SetPosition(spawnPoint.position);
            model.SetRotation(spawnPoint.rotation);
            model.SetEnabled(true);
        }
    }
}
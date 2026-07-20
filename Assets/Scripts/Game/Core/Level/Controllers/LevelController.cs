using Cysharp.Threading.Tasks;
using Game.Common.AssetsManager;
using UnityEngine;

namespace Game.Core.Level
{
    public class LevelController : ILevelController
    {
        private readonly IAssetManager _assetManager;

        private ILevelView _level;
        private string     _levelName;

        public LevelController(IAssetManager assetManager)
        {
            _assetManager = assetManager;
        }

        public async UniTask<ILevelView> LoadLevel(string address)
        {
            UnloadLevel();

            _level = await _assetManager.LoadView<ILevelView>(address);
            if (_level == null)
            {
                Debug.LogError($"Failed to load level: {address}");
                return null;
            }

            _levelName = address;
            Debug.Log($"Level loaded: {address}");

            return _level;
        }

        public void UnloadLevel()
        {
            if (_level != null)
            {
                _assetManager.UnloadAsset(_levelName);
                Debug.LogError($"Level unloaded: {_levelName}");

                _levelName = "";
            }
        }
    }
}
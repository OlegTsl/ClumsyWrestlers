using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Common.Views;
using UnityEngine;

namespace Game.Common.AssetsManager
{
    public interface IAssetManager
    {
        UniTask<T> LoadView<T>(string address, Transform parent = null) where T : class, IView;
        UniTask<T> LoadAsset<T>(string address) where T : class;
        UniTask<T> LoadAsset<T>(string address, Transform parent) where T : class;

        void UnloadAsset(string address);
        UniTask PrewarmAssets<T>(List<string> addresses) where T : class;
        void ClearCache();
    }
}
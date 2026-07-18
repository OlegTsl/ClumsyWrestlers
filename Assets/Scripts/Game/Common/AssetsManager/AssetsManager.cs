using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Common.Views;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Common.AssetsManager
{
    public class AssetManager : IAssetManager
    {
        private readonly Dictionary<string, AssetRef> _assetCache = new Dictionary<string, AssetRef>();

        public async UniTask<T> LoadView<T>(string address, Transform parent = null)
            where T : class, IView
        {
            if (!_assetCache.TryGetValue(address, out var assetRef))
            {
                var prefab = await LoadAsset<GameObject>(address);
                if (prefab == null)
                    return null;

                assetRef = _assetCache[address];
            }

            var prefabGo = (GameObject)assetRef.Object.Result;
            if (prefabGo == null)
                return null;
            
            var instance = Object.Instantiate(prefabGo, parent, false);
            if (instance == null)
                return null;


            var view = instance.GetComponent<T>();
            if (view == null)
            {
                Debug.LogError($"Loaded asset at address {address} does not implement {typeof(T)}.");
                Object.Destroy(instance);
                return null;
            }
            
            assetRef.RefCount++;

            bool disposed = false;
            view.DisposeAction = () =>
            {
                if (disposed) return;
                disposed = true;

                if (instance) Object.Destroy(instance);

                if (_assetCache.TryGetValue(address, out var ar))
                {
                    ar.RefCount--;
                    if (ar.RefCount <= 0)
                    {
                        Addressables.Release(ar.Object);
                        _assetCache.Remove(address);
                        Debug.Log($"Asset at address {address} unloaded.");
                    }
                }
            };

            return view;
        }

        public async UniTask<T> LoadAsset<T>(string address) where T : class
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _assetCache[address] = new AssetRef
                {
                    Object   = handle,
                    RefCount = 1
                };
                return handle.Result;
            }
            else
            {
                Debug.LogError($"Failed to load asset at address {address}");
                return null;
            }
        }

        public async UniTask<T> LoadAsset<T>(string address, Transform parent) where T : class
        {
            if (_assetCache.TryGetValue(address, out AssetRef assetRef))
            {
                var loadedAsset = assetRef.Object;
                var instance = GameObject.Instantiate((GameObject)loadedAsset.Result, parent, false);

                if (instance != null)
                {
                    instance.transform.SetParent(parent);
                }

                assetRef.RefCount++;
                return instance is T ? instance as T : instance.GetComponent<T>();
            }
            else
            {
                var gameObject = await LoadAsset<GameObject>(address);
                var instance   = GameObject.Instantiate(gameObject, parent, false);

                if (instance != null)
                {
                    instance.transform.SetParent(parent);
 
                    if (instance is T)
                        return instance as T;

                    var view = instance.GetComponent<T>();
                    if (view != null)
                        return view;
                    else
                    {
                        Debug.LogError($"Loaded asset at address {address} does not implement {typeof(T)} interface.");
                        Addressables.Release(instance);
                        return null;
                    }
                }
            }

            return null;
        }

        public void UnloadAsset(string address)
        {
            if (_assetCache.TryGetValue(address, out AssetRef assetRef))
            {
                assetRef.RefCount--;
                if (assetRef.RefCount <= 0)
                {
                    Addressables.Release(assetRef.Object);
                    _assetCache.Remove(address);
                    Debug.Log($"Asset at address {address} unloaded.");
                }
            }
        }

        public async UniTask PrewarmAssets<T>(List<string> addresses) where T : class
        {
            var tasks = new List<UniTask>();

            foreach (var address in addresses)
            {
                tasks.Add(LoadAsset<T>(address));
            }
            
            await UniTask.WhenAll(tasks);
        }

        public void ClearCache()
        {
            foreach (var assetRef in _assetCache.Values)
            {
                Addressables.Release(assetRef.Object);
            }
            _assetCache.Clear();
            Debug.Log("Asset cache cleared.");
        }
       
        private class AssetRef
        {
            public AsyncOperationHandle Object;
            public int RefCount;
        }
    }
}

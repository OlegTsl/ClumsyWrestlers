using Cysharp.Threading.Tasks;
using Game.Common.Views;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Common.ViewLoader
{
    public class ViewLoader<T> : IViewLoader<T> where T : IView
    {
        private readonly string _addressableKey;
        private UniTaskCompletionSource<T> _completionSource = new UniTaskCompletionSource<T>();
        private T _cachedView;
        private GameObject _cachedGameObject;

        public ViewLoader(string addressableKey)
        {
            _addressableKey = addressableKey;
            LoadViewAsync(); 
        }

        public UniTask<T> GetView()
        {
            return _completionSource.Task;
        }

        public void ResetView()
        {
            if (_cachedView != null)
            {
                Object.Destroy(_cachedGameObject);
                _cachedView = default;
            }
            
            _completionSource = new UniTaskCompletionSource<T>();
            
            LoadViewAsync();
        }
        
        private async void LoadViewAsync()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(_addressableKey);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var prefab = handle.Result;
                _cachedGameObject = Object.Instantiate(prefab);
                _cachedView = _cachedGameObject.GetComponent<T>();
                _completionSource.TrySetResult(_cachedView);
            }
            else
            {
                _completionSource.TrySetException(new System.Exception($"Failed to load asset with key {_addressableKey}"));
            }
        }
    }
}
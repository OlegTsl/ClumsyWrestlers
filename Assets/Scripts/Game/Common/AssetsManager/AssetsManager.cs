using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Common.Views;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Game.Common.AssetsManager
{
    public sealed class AssetManager : IAssetManager
    {
        private readonly Dictionary<AssetKey, AssetEntry> _assetEntries = new();

        private bool _isDisposed;

        public async UniTask<IAssetLease<T>> LoadAssetAsync<T>(
            string address,
            CancellationToken cancellationToken
        ) where T : Object
        {
            ThrowIfDisposed();

            AssetKey key = new(address, typeof(T));
            AssetEntry entry = GetOrCreateEntry<T>(key, address);
            entry.PendingConsumers++;

            try
            {
                await entry.Handle.ToUniTask(cancellationToken: cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                ThrowIfDisposed();

                if (entry.Handle.Status != AsyncOperationStatus.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"[AssetManager] Failed to load '{address}' as {typeof(T).Name}.");
                }

                if (!_assetEntries.TryGetValue(key, out AssetEntry currentEntry) ||
                    !ReferenceEquals(currentEntry, entry))
                {
                    throw new OperationCanceledException(cancellationToken);
                }

                entry.ActiveLeases++;
                return new AssetLease<T>(this, key, entry, (T)entry.Handle.Result);
            }
            finally
            {
                entry.PendingConsumers--;
                TryReleaseUnusedEntry(key, entry);
            }
        }

        public async UniTask<IViewLease<TView>> InstantiateViewAsync<TView>(
            string address,
            Transform parent,
            CancellationToken cancellationToken
        ) where TView : class, IView
        {
            IAssetLease<ViewPrefabAsset> prefabLease = null;
            MonoBehaviour instance = null;

            try
            {
                prefabLease = await LoadAssetAsync<ViewPrefabAsset>(address, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (!(prefabLease.Asset.ViewPrefab is TView))
                {
                    throw new InvalidOperationException(
                        $"[AssetManager] View asset '{address}' is not {typeof(TView).Name}.");
                }

                instance = Object.Instantiate(
                    prefabLease.Asset.ViewPrefab,
                    parent,
                    false);
                return new ViewLease<TView>(prefabLease, instance, (TView)(object)instance);
            }
            catch
            {
                if (instance != null)
                {
                    Object.Destroy(instance.gameObject);
                }

                prefabLease?.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            foreach (AssetEntry entry in _assetEntries.Values)
            {
                if (entry.Handle.IsValid())
                {
                    Addressables.Release(entry.Handle);
                }

                entry.IsReleased = true;
            }

            _assetEntries.Clear();
        }

        private AssetEntry GetOrCreateEntry<T>(AssetKey key, string address) where T : Object
        {
            if (_assetEntries.TryGetValue(key, out AssetEntry entry))
            {
                return entry;
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
            entry = new AssetEntry(handle);
            _assetEntries.Add(key, entry);
            return entry;
        }

        private void Release(AssetKey key, AssetEntry entry)
        {
            if (entry.IsReleased || entry.ActiveLeases <= 0)
            {
                return;
            }

            entry.ActiveLeases--;
            TryReleaseUnusedEntry(key, entry);
        }

        private void TryReleaseUnusedEntry(AssetKey key, AssetEntry entry)
        {
            if (_isDisposed || entry.IsReleased || entry.PendingConsumers > 0 ||
                entry.ActiveLeases > 0)
            {
                return;
            }

            if (!_assetEntries.TryGetValue(key, out AssetEntry currentEntry) ||
                !ReferenceEquals(currentEntry, entry))
            {
                return;
            }

            _assetEntries.Remove(key);
            if (entry.Handle.IsValid())
            {
                Addressables.Release(entry.Handle);
            }

            entry.IsReleased = true;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(AssetManager));
            }
        }

        private readonly struct AssetKey : IEquatable<AssetKey>
        {
            private readonly string _address;
            private readonly Type _assetType;

            public AssetKey(string address, Type assetType)
            {
                _address = address;
                _assetType = assetType;
            }

            public bool Equals(AssetKey other)
                => string.Equals(_address, other._address, StringComparison.Ordinal) &&
                   _assetType == other._assetType;

            public override bool Equals(object obj)
                => obj is AssetKey other && Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((_address != null ? _address.GetHashCode() : 0) * 397) ^
                           (_assetType != null ? _assetType.GetHashCode() : 0);
                }
            }
        }

        private sealed class AssetEntry
        {
            public AsyncOperationHandle Handle { get; }
            public int PendingConsumers { get; set; }
            public int ActiveLeases { get; set; }
            public bool IsReleased { get; set; }

            public AssetEntry(AsyncOperationHandle handle)
                => Handle = handle;
        }

        private sealed class AssetLease<T> : IAssetLease<T> where T : Object
        {
            private AssetManager _owner;
            private readonly AssetKey _key;
            private readonly AssetEntry _entry;

            public T Asset { get; }

            public AssetLease(AssetManager owner, AssetKey key, AssetEntry entry, T asset)
            {
                _owner = owner;
                _key = key;
                _entry = entry;
                Asset = asset;
            }

            public void Dispose()
            {
                AssetManager owner = _owner;
                if (owner == null)
                {
                    return;
                }

                _owner = null;
                owner.Release(_key, _entry);
            }
        }

        private sealed class ViewLease<TView> : IViewLease<TView>
            where TView : class, IView
        {
            private IAssetLease<ViewPrefabAsset> _prefabLease;

            public TView View { get; }
            public GameObject GameObject { get; private set; }

            public ViewLease(
                IAssetLease<ViewPrefabAsset> prefabLease,
                MonoBehaviour instance,
                TView view
            )
            {
                _prefabLease = prefabLease;
                GameObject = instance.gameObject;
                View = view;
            }

            public void Dispose()
            {
                IAssetLease<ViewPrefabAsset> prefabLease = _prefabLease;
                if (prefabLease == null)
                {
                    return;
                }

                _prefabLease = null;
                GameObject gameObject = GameObject;
                GameObject = null;
                if (gameObject != null)
                {
                    Object.Destroy(gameObject);
                }

                prefabLease.Dispose();
            }
        }
    }
}

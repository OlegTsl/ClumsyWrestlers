using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Common.Views;
using UnityEngine;

namespace Game.Common.AssetsManager
{
    public interface IAssetManager : IDisposable
    {
        UniTask<IAssetLease<T>> LoadAssetAsync<T>(
            string address,
            CancellationToken cancellationToken
        ) where T : UnityEngine.Object;

        UniTask<IViewLease<TView>> InstantiateViewAsync<TView>(
            string address,
            Transform parent,
            CancellationToken cancellationToken
        ) where TView : class, IView;
    }
}

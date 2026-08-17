using System;

namespace Game.Common.AssetsManager
{
    public interface IAssetLease<out T> : IDisposable
    {
        T Asset { get; }
    }
}

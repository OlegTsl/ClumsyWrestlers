using System;
using Game.Common.Views;
using UnityEngine;

namespace Game.Common.AssetsManager
{
    public interface IViewLease<out TView> : IDisposable where TView : class, IView
    {
        TView View { get; }
        GameObject GameObject { get; }
    }
}

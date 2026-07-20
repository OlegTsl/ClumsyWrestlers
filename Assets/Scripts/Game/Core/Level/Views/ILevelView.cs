using Game.Common.Views;
using UnityEngine;

namespace Game.Core.Level
{
    public interface ILevelView : IView
    {
        Transform PlayerSpawnPoint { get; }
    }
}
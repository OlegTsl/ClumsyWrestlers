using UnityEngine;

namespace Game.Core.Level
{
    public interface ILevelArenaData
    {
        Vector3 SafePosition { get; }
        float EliminationHeight { get; }
    }
}

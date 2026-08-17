using UnityEngine;

namespace Game.Core.Level
{
    public readonly struct LevelSpawnPoint
    {
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public LevelSpawnPoint(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}

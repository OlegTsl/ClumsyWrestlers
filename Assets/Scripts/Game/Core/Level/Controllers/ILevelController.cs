using Cysharp.Threading.Tasks;

namespace Game.Core.Level
{
    public interface ILevelController
    {
        UniTask LoadLevel(string address);
        void UnloadLevel();

        bool IsLevelLoaded();

        UniTask SpawnPlayer(string name);
        UniTask SpawnEnemy(string name);
    }
}
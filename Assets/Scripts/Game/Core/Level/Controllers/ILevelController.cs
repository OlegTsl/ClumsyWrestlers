using Cysharp.Threading.Tasks;

namespace Game.Core.Level
{
    public interface ILevelController
    {
        UniTask LoadLevel(string address);
        void UnloadLevel();

        bool IsLevelLoaded();

        UniTask SpawnCharacter(string name, bool isPlayer);
    }
}
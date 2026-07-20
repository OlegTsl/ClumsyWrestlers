using Cysharp.Threading.Tasks;

namespace Game.Core.Level
{
    public interface ILevelController
    {
        UniTask<ILevelView> LoadLevel(string address);
        void UnloadLevel();
    }
}
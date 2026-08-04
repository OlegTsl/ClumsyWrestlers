using Cysharp.Threading.Tasks;
using Game.Core.Character;

namespace Game.Core.Level
{
    public interface ILevelController
    {
        UniTask LoadLevel(string address);
        void UnloadLevel();
        bool IsLevelLoaded();

        void SpawnCharacter(ICharacterModel model, bool isPlayer);
    }
}
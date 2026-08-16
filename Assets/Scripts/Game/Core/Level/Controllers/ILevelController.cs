using Cysharp.Threading.Tasks;
using Game.Core.Character;

namespace Game.Core.Level
{
    public interface ILevelController
    {
        ILevelModel Level { get; }

        UniTask LoadLevel(string address);
        void UnloadLevel();
        bool IsLevelLoaded();

        void SpawnCharacter(ICharacterModel model, bool isPlayer);
    }
}

using Cysharp.Threading.Tasks;

namespace Game.Core.Character
{
    public interface ICharacterBuilder
    {
        UniTask<ICharacterModel> BuidCharacter(string name);
    }
}
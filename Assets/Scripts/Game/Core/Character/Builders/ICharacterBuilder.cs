using Cysharp.Threading.Tasks;

namespace Game.Core.Character
{
    public interface ICharacterBuilder
    {
        UniTask<ICharacter> BuidCharacter(string name);
    }
}
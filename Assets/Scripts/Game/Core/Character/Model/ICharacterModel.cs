using Game.Core.Entities;

namespace Game.Core.Character
{
    public interface ICharacterModel
    {
        EntityId CharacterID { get; }
        CharacterData Data { get; }
    }
}

using System;

namespace Game.Core.Character
{
    public interface ICharactersController
    {
        void AddCharacter(ICharacterContext context);
        void RemoveCharacter(ICharacterContext context);
        void RemoveAllCharacters();
        void SetCharacterEnabled(Guid characterID, bool enabled);
    }
}
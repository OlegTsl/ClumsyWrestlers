using System;
using System.Collections.Generic;
using Game.Core.Entities;

namespace Game.Core.Character
{
    public interface ICharacterContext
    {
        IReadOnlyList<ICharacterModel> AllCharacters { get; }

        event Action<EntityId> OnCharacterAdded;
        event Action<EntityId> OnCharacterRemoved;

        void AddCharacter(ICharacterModel character);
        void RemoveCharacter(EntityId characterId);
        ICharacterModel GetModel(EntityId characterId);
    }
}

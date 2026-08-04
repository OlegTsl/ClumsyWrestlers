using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Character
{
    public interface ICharacterContext
    { 
        void            AddCharacter(ICharacterModel character);
        void            RemoveCharacter(Guid characterID);
        ICharacterModel GetModel(Guid characterID);
        ICharacterModel GetModel(Collider collider);

        IReadOnlyList<ICharacterModel> AllCharacters { get; }
        event Action<Guid> OnCharacterAdded;
        event Action<Guid> OnCharacterRemoved;
    }
}
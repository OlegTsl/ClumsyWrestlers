using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Character
{
    public interface ICharacterContext
    { 
        void       AddCharacter(ICharacter character);
        void       RemoveCharacter(Guid characterID);
        ICharacter GetCharacter(Guid characterID);
        ICharacter GetCharacter(Collider collider);

        IReadOnlyList<ICharacter> AllCharacters { get; }
        event Action<Guid> OnCharacterAdded;
        event Action<Guid> OnCharacterRemoved;
    }
}
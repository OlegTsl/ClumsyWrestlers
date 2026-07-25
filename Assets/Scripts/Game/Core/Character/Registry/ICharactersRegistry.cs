using System;
using UnityEngine;

namespace Game.Core.Character
{
    public interface ICharactersRegistry
    {
        void Register(ICharacterContext context);
        void Unregister(ICharacterContext context);

        Guid GetByCollider(Collider collider);
        ICharacterContext GetContext(Guid characterID);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Character
{
    public class CharactersRegistry : ICharactersRegistry
    {
        private readonly Dictionary<Collider, Guid>          _hitBoxes   = new();
        private readonly Dictionary<Guid, ICharacterContext> _characters = new();

        public void Register(ICharacterContext context)
        {
            _hitBoxes[context.View.HitBox]   = context.CharacterId;
            _characters[context.CharacterId] = context;
        }

        public void Unregister(ICharacterContext context)
        {
            _characters.Remove(context.CharacterId);
            _hitBoxes.Remove(context.View.HitBox);
        }

        public Guid GetByCollider(Collider collider)
        {
            _hitBoxes.TryGetValue(collider, out var guid);
            return guid;
        }

        public ICharacterContext GetContext(Guid characterID)
        {
            _characters.TryGetValue(characterID, out var context);
            return context;
        }
    }
}
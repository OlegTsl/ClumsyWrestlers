using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Character
{
    public sealed class CharacterContext : ICharacterContext
    {
        private readonly Dictionary<Guid, ICharacter>     _characters     = new();
        private readonly Dictionary<Collider, ICharacter> _colliders      = new();
        private readonly List<ICharacter>                 _charactersList = new();

        public IReadOnlyList<ICharacter> AllCharacters
            => _charactersList;

        public event Action<Guid> OnCharacterAdded;
        public event Action<Guid> OnCharacterRemoved;

        public void AddCharacter(ICharacter character)
        {
            _characters[character.CharacterID] = character;
            _colliders[character.Hitbox]       = character;
            _charactersList.Add(character);

            OnCharacterAdded?.Invoke(character.CharacterID);
        }

        public void RemoveCharacter(Guid characterID)
        {
            if (_characters.TryGetValue(characterID, out var character))
            {
                _characters.Remove(characterID);
                _colliders.Remove(character.Hitbox);
                _charactersList.Remove(character);

                OnCharacterRemoved?.Invoke(character.CharacterID);
            }
        }

        public ICharacter GetCharacter(Guid characterID)
        {
            _characters.TryGetValue(characterID, out var character);
            return character;
        }

        public ICharacter GetCharacter(Collider collider)
        {
            _colliders.TryGetValue(collider, out var character);
            return character;
        }
    }
}
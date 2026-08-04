using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Character
{
    public sealed class CharacterContext : ICharacterContext
    {
        private readonly Dictionary<Guid, ICharacterModel>     _characters     = new();
        private readonly Dictionary<Collider, ICharacterModel> _colliders      = new();
        private readonly List<ICharacterModel>                 _charactersList = new();

        public IReadOnlyList<ICharacterModel> AllCharacters
            => _charactersList;

        public event Action<Guid> OnCharacterAdded;
        public event Action<Guid> OnCharacterRemoved;

        public void AddCharacter(ICharacterModel character)
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

        public ICharacterModel GetModel(Guid characterID)
        {
            _characters.TryGetValue(characterID, out var model);
            return model;
        }

        public ICharacterModel GetModel(Collider collider)
        {
            _colliders.TryGetValue(collider, out var model);
            return model;
        }
    }
}
using System;
using System.Collections.Generic;
using Game.Core.Entities;

namespace Game.Core.Character
{
    public sealed class CharacterContext : ICharacterContext
    {
        private readonly Dictionary<EntityId, ICharacterModel> _characters = new();
        private readonly List<ICharacterModel> _charactersList = new();

        public IReadOnlyList<ICharacterModel> AllCharacters => _charactersList;

        public event Action<EntityId> OnCharacterAdded;
        public event Action<EntityId> OnCharacterRemoved;

        public void AddCharacter(ICharacterModel character)
        {
            _characters.Add(character.CharacterID, character);
            _charactersList.Add(character);
            OnCharacterAdded?.Invoke(character.CharacterID);
        }

        public void RemoveCharacter(EntityId characterId)
        {
            if (!_characters.TryGetValue(characterId, out ICharacterModel character))
            {
                return;
            }

            _characters.Remove(characterId);
            _charactersList.Remove(character);
            OnCharacterRemoved?.Invoke(characterId);
        }

        public ICharacterModel GetModel(EntityId characterId)
        {
            _characters.TryGetValue(characterId, out ICharacterModel model);
            return model;
        }
    }
}

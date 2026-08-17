using System;
using System.Collections.Generic;
using Game.Core.Character;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Systems
{
    public sealed class CharacterControlStateRegistry : IDisposable
    {
        private readonly ICharacterContext _characters;
        private readonly Dictionary<EntityId, CharacterControlState> _states = new(16);

        public CharacterControlStateRegistry(ICharacterContext characters)
        {
            _characters = characters;
            _characters.OnCharacterAdded += Register;
            _characters.OnCharacterRemoved += Unregister;

            var existingCharacters = _characters.AllCharacters;
            for (int i = 0; i < existingCharacters.Count; i++)
            {
                Register(existingCharacters[i].CharacterID);
            }
        }

        public bool TryGet(
            EntityId characterId,
            out CharacterControlState state)
            => _states.TryGetValue(characterId, out state);

        private void Register(EntityId characterId)
        {
            if (!_states.ContainsKey(characterId))
            {
                _states.Add(characterId, new CharacterControlState());
            }
        }

        private void Unregister(EntityId characterId)
            => _states.Remove(characterId);

        public void Dispose()
        {
            _characters.OnCharacterAdded -= Register;
            _characters.OnCharacterRemoved -= Unregister;
            _states.Clear();
        }
    }

    public sealed class CharacterControlState
    {
        public Vector3 MoveDirection { get; set; }
    }
}

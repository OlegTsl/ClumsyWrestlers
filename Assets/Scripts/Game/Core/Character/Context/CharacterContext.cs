using System;
using System.Collections.Generic;
using Game.Common.Input;
using Game.Core.Events;

namespace Game.Core.Character
{
    public sealed class CharacterContext : ICharacterContext
    {
        private readonly Dictionary<Type, object> _systems = new();
 
        public ICharacterView  View        { get; }
        public Guid            CharacterId { get; }
        public InputEventsBus  InputEvents { get; }
        public GameEventsBus   GameEvents  { get; }
 
        public CharacterContext(
            ICharacterView view,
            Guid           characterId,
            InputEventsBus inputEvents,
            GameEventsBus  gameEvents)
        {
            View        = view;
            CharacterId = characterId;
            InputEvents = inputEvents;
            GameEvents  = gameEvents;
        }
 
        public void AddSystem<T>(T system) where T : class
            => _systems[typeof(T)] = system;

        public T GetSystem<T>() where T : class
        {
            if (_systems.TryGetValue(typeof(T), out var system))
                return (T)system;
 
            throw new InvalidOperationException($"System {typeof(T).Name} not found! ");
        }
  
        public bool TryGetSystem<T>(out T system) where T : class
        {
            if (_systems.TryGetValue(typeof(T), out var raw))
            {
                system = (T)raw;
                return true;
            }
 
            system = null;
            return false;
        }
    }
}
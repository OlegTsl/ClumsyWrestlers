using System;
using Game.Common.Input;
using Game.Core.Events;

namespace Game.Core.Character
{
    public interface ICharacterContext
    {
        ICharacterView  View        { get; }
        Guid            CharacterId { get; }
        InputEventsBus  InputEvents { get; }
        GameEventsBus   GameEvents  { get; }
 
        void AddSystem<T>(T system) where T : class;
        T    GetSystem<T>() where T : class;
        bool TryGetSystem<T>(out T system) where T : class;
    }
}
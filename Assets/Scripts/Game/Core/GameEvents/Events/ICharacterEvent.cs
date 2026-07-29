using System;

namespace Game.Core.GameEvents
{
    public interface ICharacterEvent
    {
        Guid CharacterID { get; }
    }
}
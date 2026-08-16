using System;

namespace Game.Core.GameEvents
{
    public readonly struct OnFallEvent
    {
        public readonly Guid CharacterID { get; }
        public OnFallEvent(Guid characterID)
            => CharacterID = characterID;
    }
}
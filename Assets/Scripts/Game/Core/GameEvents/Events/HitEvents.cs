using System;

namespace Game.Core.GameEvents
{
    public readonly struct OnHitEvent
    {
        public readonly Guid CharacterID { get; }
        public OnHitEvent(Guid characterID)
            => CharacterID = characterID;
    }
}
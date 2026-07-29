using System;

namespace Game.Core.GameEvents
{
    public readonly struct OnDamageEvent : ICharacterEvent
    {
        public Guid CharacterID { get; }
        public OnDamageEvent(Guid characterID)
            => CharacterID = characterID;
    }
}
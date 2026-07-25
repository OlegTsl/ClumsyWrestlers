using System;

namespace Game.Core.Events
{
    public readonly struct OnPunchStartedEvent { }
    public readonly struct OnPunchEndedEvent   { }
    public readonly struct OnPunchLandedEvent
    {
        public Guid CharacterID { get; }
        public OnPunchLandedEvent(Guid characterID)
            => CharacterID = characterID;
    }
}
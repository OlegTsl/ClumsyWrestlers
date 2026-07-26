using System;

namespace Game.Core.Events
{
    public readonly struct OnPunchStartedEvent      { }
    public readonly struct OnPunchEndedEvent        { }
    public readonly struct OnPowerPunchStartedEvent { }
    public readonly struct OnPowerPunchEndedEvent   { }
    public readonly struct OnHitLandedEvent
    {
        public Guid CharacterID { get; }
        public OnHitLandedEvent(Guid characterID)
            => CharacterID = characterID;
    }
}
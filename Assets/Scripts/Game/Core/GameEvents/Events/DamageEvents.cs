using System;

namespace Game.Core.GameEvents
{
    public readonly struct OnDamageEvent : ICharacterEvent
    {
        public readonly Guid CharacterID { get; }
        public readonly Guid TargetID    { get; }
        public OnDamageEvent(Guid characterID, Guid targetID)
        {
            CharacterID = characterID;
            TargetID    = targetID;
        }
    }
}
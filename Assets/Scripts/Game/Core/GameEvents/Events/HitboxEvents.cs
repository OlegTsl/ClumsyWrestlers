using System;

namespace Game.Core.GameEvents
{
    public readonly struct OnHitboxEnabledEvent : ICharacterEvent
    {
        public Guid CharacterID { get; }
        public bool Enabled     { get; }
        public OnHitboxEnabledEvent(Guid characterID, bool enabled)
        {
            CharacterID = characterID;
            Enabled     = enabled;
        }
    }
}
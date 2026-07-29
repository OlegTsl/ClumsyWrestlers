using System;

namespace Game.Core.GameEvents
{
    public readonly struct OnPowerAttackEvent : ICharacterEvent
    {
        public Guid CharacterID { get; }
        public OnPowerAttackEvent(Guid characterID)
            => CharacterID = characterID;
    }

    public readonly struct OnPowerAttackStartedEvent : ICharacterEvent
    {
        public Guid CharacterID { get; }
        public OnPowerAttackStartedEvent(Guid characterID)
            => CharacterID = characterID;
    }

    public readonly struct OnPowerAttackEndedEvent : ICharacterEvent
    {
        public Guid CharacterID { get; }
        public OnPowerAttackEndedEvent(Guid characterID)
            => CharacterID = characterID;
    }
}
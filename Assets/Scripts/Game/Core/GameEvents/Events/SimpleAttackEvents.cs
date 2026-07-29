using System;

namespace Game.Core.GameEvents
{
    public readonly struct OnSimpleAttackEvent : ICharacterEvent
    {
        public Guid CharacterID { get; }
        public OnSimpleAttackEvent(Guid characterID)
            => CharacterID = characterID;
    }

    public readonly struct OnSimpleAttackStartedEvent : ICharacterEvent
    {
        public Guid CharacterID { get; }
        public OnSimpleAttackStartedEvent(Guid characterID)
            => CharacterID = characterID;
    }

    public readonly struct OnSimpleAttackEndedEvent : ICharacterEvent
    {
        public Guid CharacterID { get; }
        public OnSimpleAttackEndedEvent(Guid characterID)
            => CharacterID = characterID;
    }
}
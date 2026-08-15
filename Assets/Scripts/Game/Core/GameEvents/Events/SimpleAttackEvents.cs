using System;

namespace Game.Core.GameEvents
{
    public readonly struct OnSimpleAttackInputEvent : ICharacterEvent
    {
        public Guid CharacterID { get; }
        public bool IsPressed { get; }

        public OnSimpleAttackInputEvent(Guid characterID, bool isPressed)
        {
            CharacterID = characterID;
            IsPressed = isPressed;
        }
    }

    public readonly struct OnSimpleAttackStartedEvent : ICharacterEvent
    {
        public Guid CharacterID { get; }
        public bool IsMirrored { get; }

        public OnSimpleAttackStartedEvent(
            Guid characterID,
            bool isMirrored
        )
        {
            CharacterID = characterID;
            IsMirrored = isMirrored;
        }
    }

    public readonly struct OnSimpleAttackEndedEvent : ICharacterEvent
    {
        public Guid CharacterID { get; }

        public OnSimpleAttackEndedEvent(Guid characterID)
        {
            CharacterID = characterID;
        }
    }
}

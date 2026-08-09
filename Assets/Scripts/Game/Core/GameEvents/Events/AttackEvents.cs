using System;

namespace Game.Core.GameEvents
{
    public enum AttackType
    {
        Simple,
        Power
    }

    public readonly struct OnAttackEvent : ICharacterEvent
    {
        public readonly Guid       CharacterID { get; }
        public readonly AttackType Type        { get; }
        public OnAttackEvent(Guid characterID, AttackType type)
        {
            CharacterID = characterID;
            Type        = type;
        }
    }

    public readonly struct OnAttackStartedEvent : ICharacterEvent
    {
        public readonly Guid       CharacterID { get; }
        public readonly AttackType Type        { get; }
        public OnAttackStartedEvent(Guid characterID, AttackType type)
        {
            CharacterID = characterID;
            Type        = type;
        }
    }

    public readonly struct OnAttackEndedEvent : ICharacterEvent
    {
        public readonly Guid       CharacterID { get; }
        public readonly AttackType Type        { get; }
        public OnAttackEndedEvent(Guid characterID, AttackType type)
        {
            CharacterID = characterID;
            Type        = type;
        }
    }
}
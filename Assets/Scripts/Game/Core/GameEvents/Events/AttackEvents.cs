using System;
using UnityEngine;

namespace Game.Core.GameEvents
{
    public enum AttackType
    {
        Simple,
        Power
    }

    public readonly struct OnPowerAttackRequestedEvent : ICharacterEvent
    {
        public readonly Guid CharacterID { get; }

        public OnPowerAttackRequestedEvent(Guid characterID)
            => CharacterID = characterID;
    }

    public readonly struct OnPowerAttackStartedEvent : ICharacterEvent
    {
        public Guid CharacterID { get; }
        public AnimationClip AnimationClip { get; }

        public OnPowerAttackStartedEvent(
            Guid characterID,
            AnimationClip animationClip
        )
        {
            CharacterID = characterID;
            AnimationClip = animationClip;
        }
    }

    public enum PowerAttackEndReason
    {
        Completed,
        Interrupted
    }

    public readonly struct OnPowerAttackEndedEvent : ICharacterEvent
    {
        public Guid CharacterID { get; }
        public PowerAttackEndReason Reason { get; }

        public OnPowerAttackEndedEvent(
            Guid characterID,
            PowerAttackEndReason reason
        )
        {
            CharacterID = characterID;
            Reason = reason;
        }
    }
}

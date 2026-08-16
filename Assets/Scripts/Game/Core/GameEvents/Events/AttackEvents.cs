using System;
using UnityEngine;

namespace Game.Core.GameEvents
{
    public enum AttackType
    {
        Simple,
        Power
    }

    public readonly struct OnPowerAttackRequestedEvent
    {
        public readonly Guid CharacterID { get; }
        public OnPowerAttackRequestedEvent(Guid characterID)
            => CharacterID = characterID;
    }

    public readonly struct OnPowerAttackStartedEvent
    {
        public Guid          CharacterID   { get; }
        public AnimationClip AnimationClip { get; }

        public OnPowerAttackStartedEvent(
            Guid          characterID,
            AnimationClip animationClip
        )
        {
            CharacterID   = characterID;
            AnimationClip = animationClip;
        }
    }

    public readonly struct OnPowerAttackEndedEvent
    {
        public Guid CharacterID { get; }
        public OnPowerAttackEndedEvent(Guid characterID)
            => CharacterID = characterID;
    }
}

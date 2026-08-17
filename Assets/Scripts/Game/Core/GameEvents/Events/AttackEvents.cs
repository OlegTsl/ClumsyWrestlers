using Game.Core.Entities;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.GameEvents
{
    public enum AttackType
    {
        Simple,
        Power
    }

    public readonly struct OnPowerAttackRequestedEvent
    {
        public EntityId CharacterID { get; }

        public OnPowerAttackRequestedEvent(EntityId characterID)
            => CharacterID = characterID;
    }

    public readonly struct OnPowerAttackStartedEvent
    {
        public EntityId CharacterID { get; }
        public AnimationClip AnimationClip { get; }

        public OnPowerAttackStartedEvent(
            EntityId characterID,
            AnimationClip animationClip
        )
        {
            CharacterID = characterID;
            AnimationClip = animationClip;
        }
    }

    public readonly struct OnPowerAttackEndedEvent
    {
        public EntityId CharacterID { get; }

        public OnPowerAttackEndedEvent(EntityId characterID)
            => CharacterID = characterID;
    }
}

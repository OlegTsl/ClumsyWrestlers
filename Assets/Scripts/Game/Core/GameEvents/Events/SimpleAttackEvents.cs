using Game.Core.Character;
using Game.Core.Entities;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.GameEvents
{
    public readonly struct OnSimpleAttackInputEvent
    {
        public EntityId CharacterID { get; }
        public bool IsPressed { get; }

        public OnSimpleAttackInputEvent(EntityId characterID, bool isPressed)
        {
            CharacterID = characterID;
            IsPressed = isPressed;
        }
    }

    public readonly struct OnSimpleAttackStartedEvent
    {
        public EntityId CharacterID { get; }
        public bool IsMirrored { get; }

        public OnSimpleAttackStartedEvent(EntityId characterID, bool isMirrored)
        {
            CharacterID = characterID;
            IsMirrored = isMirrored;
        }
    }

    public readonly struct OnSimpleAttackEndedEvent
    {
        public EntityId CharacterID { get; }

        public OnSimpleAttackEndedEvent(EntityId characterID)
            => CharacterID = characterID;
    }

    public readonly struct OnAttackHandIkEvent
    {
        public EntityId CharacterID { get; }
        public AttackHand Hand { get; }
        public Vector3 Position { get; }
        public float Weight { get; }

        public OnAttackHandIkEvent(
            EntityId characterID,
            AttackHand hand,
            Vector3 position,
            float weight
        )
        {
            CharacterID = characterID;
            Hand = hand;
            Position = position;
            Weight = weight;
        }
    }

    public readonly struct OnAttackHandIkClearedEvent
    {
        public EntityId CharacterID { get; }

        public OnAttackHandIkClearedEvent(EntityId characterID)
            => CharacterID = characterID;
    }
}

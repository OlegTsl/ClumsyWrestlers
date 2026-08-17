using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.GameEvents
{
    public readonly struct OnMoveEvent
    {
        public EntityId CharacterID { get; }
        public Vector3 Direction { get; }

        public OnMoveEvent(EntityId characterID, Vector3 direction)
        {
            CharacterID = characterID;
            Direction = direction;
        }
    }

    public readonly struct OnJumpEvent
    {
        public EntityId CharacterID { get; }

        public OnJumpEvent(EntityId characterID)
            => CharacterID = characterID;
    }

    public readonly struct OnForceEvent
    {
        public EntityId CharacterID { get; }
        public Vector3 Force { get; }

        public OnForceEvent(EntityId characterID, Vector3 force)
        {
            CharacterID = characterID;
            Force = force;
        }
    }
}

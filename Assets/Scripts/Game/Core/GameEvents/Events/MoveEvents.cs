using System;
using UnityEngine;

namespace Game.Core.GameEvents
{
    public readonly struct OnMoveEvent : ICharacterEvent
    {
        public Guid    CharacterID { get; }
        public Vector3 Direction   { get; }

        public OnMoveEvent(Guid characterID, Vector3 direction)
        {
            CharacterID = characterID;
            Direction   = direction;
        }
    }

    public readonly struct OnJumpEvent : ICharacterEvent
    {
        public Guid CharacterID { get; }
        public OnJumpEvent(Guid characterID)
            => CharacterID = characterID;
    }
}
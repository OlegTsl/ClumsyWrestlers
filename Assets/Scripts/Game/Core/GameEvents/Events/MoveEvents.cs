using System;
using UnityEngine;

namespace Game.Core.GameEvents
{
    public readonly struct OnMoveEvent : ICharacterEvent
    {
        public readonly Guid    CharacterID { get; }
        public readonly Vector3 Direction   { get; }

        public OnMoveEvent(Guid characterID, Vector3 direction)
        {
            CharacterID = characterID;
            Direction   = direction;
        }
    }

    public readonly struct OnJumpEvent : ICharacterEvent
    {
        public readonly Guid CharacterID { get; }
        public OnJumpEvent(Guid characterID)
            => CharacterID = characterID;
    }

    public readonly struct OnForceEvent : ICharacterEvent
    {
        public readonly Guid    CharacterID  { get; }
        public readonly Vector3 Force        { get; }
        public readonly bool    Controllable { get; }
        public OnForceEvent(Guid characterID, Vector3 force, bool controllable)
        {
            CharacterID  = characterID;
            Force        = force;
            Controllable = controllable;
        }
    }
}
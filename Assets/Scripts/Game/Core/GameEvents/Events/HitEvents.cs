using System;
using UnityEngine;

namespace Game.Core.GameEvents
{
    public readonly struct HitData
    {
        public AttackType AttackType { get; }
        public Guid       AttackerID { get; }
        public Guid       TargetID   { get; }
        public Vector3    Direction  { get; }

        public HitData(AttackType attackType, Guid attackerID, Guid targetID, Vector3 direction)
        {
            AttackType = attackType;
            AttackerID = attackerID;
            TargetID   = targetID;
            Direction  = direction;
        }
    }

    public readonly struct OnHitEvent
    {
        public readonly Guid CharacterID { get; }
        public OnHitEvent(Guid characterID)
            => CharacterID = characterID;
    }

    public readonly struct OnHitDetectedEvent
    {
        public HitData Hit { get; }
        public OnHitDetectedEvent(Guid attackerID, Guid targetID, Vector3 direction)
        {
            Hit = new HitData(AttackType.Simple,
                attackerID, targetID, direction);
        }
    }

    public readonly struct OnHitResolvedEvent
    {
        public HitData Hit { get; }
        public OnHitResolvedEvent(HitData hit)
            => Hit = hit;
    }
}

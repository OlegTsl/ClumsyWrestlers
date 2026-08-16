using System;
using UnityEngine;

namespace Game.Core.GameEvents
{
    public enum HitObjectType
    {
        Character,
        Environment
    }

    public readonly struct HitData
    {
        public HitObjectType SourceType      { get; }
        public Guid          SourceID        { get; }
        public HitObjectType TargetType      { get; }
        public Guid          TargetID        { get; }
        public AttackType    AttackType      { get; }
        public Vector3       Direction       { get; }
        public Vector3       ImpactVelocity  { get; }
        public Vector3       Force           { get; }

        public HitData(
            HitObjectType sourceType,
            Guid          sourceID,
            HitObjectType targetType,
            Guid          targetID,
            AttackType    attackType,
            Vector3       direction,
            Vector3       impactVelocity = default
        )
        {
            SourceType     = sourceType;
            SourceID       = sourceID;
            TargetType     = targetType;
            TargetID       = targetID;
            AttackType     = attackType;
            Direction      = direction;
            ImpactVelocity = impactVelocity;
            Force          = Vector3.zero;
        }

        private HitData(HitData hit, Vector3 force)
        {
            SourceType     = hit.SourceType;
            SourceID       = hit.SourceID;
            TargetType     = hit.TargetType;
            TargetID       = hit.TargetID;
            AttackType     = hit.AttackType;
            Direction      = hit.Direction;
            ImpactVelocity = hit.ImpactVelocity;
            Force          = force;
        }

        public HitData WithForce(Vector3 force)
            => new(this, force);
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
        public OnHitDetectedEvent(HitData hit)
            => Hit = hit;
    }

    public readonly struct OnHitResolvedEvent
    {
        public HitData Hit { get; }
        public OnHitResolvedEvent(HitData hit)
            => Hit = hit;
    }
}

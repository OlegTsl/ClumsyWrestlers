using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.GameEvents
{
    public enum HitObjectType
    {
        Character,
        LevelEntity
    }

    public readonly struct HitData
    {
        public HitObjectType SourceType      { get; }
        public EntityId      SourceID        { get; }
        public HitObjectType TargetType      { get; }
        public EntityId      TargetID        { get; }
        public AttackType    AttackType      { get; }
        public Vector3       Direction       { get; }
        public float         KnockbackForce  { get; }
        public float         KnockbackHeight { get; }
        public Vector3       ImpactVelocity  { get; }
        public Vector3       Force           { get; }

        public HitData(
            HitObjectType sourceType,
            EntityId      sourceID,
            HitObjectType targetType,
            EntityId      targetID,
            AttackType    attackType,
            Vector3       direction,
            float         knockbackForce,
            float         knockbackHeight,
            Vector3       impactVelocity = default
        )
        {
            SourceType      = sourceType;
            SourceID        = sourceID;
            TargetType      = targetType;
            TargetID        = targetID;
            AttackType      = attackType;
            Direction       = direction;
            KnockbackForce  = knockbackForce;
            KnockbackHeight = knockbackHeight;
            ImpactVelocity  = impactVelocity;
            Force           = Vector3.zero;
        }

        private HitData(in HitData hit, Vector3 force)
        {
            SourceType      = hit.SourceType;
            SourceID        = hit.SourceID;
            TargetType      = hit.TargetType;
            TargetID        = hit.TargetID;
            AttackType      = hit.AttackType;
            Direction       = hit.Direction;
            KnockbackForce  = hit.KnockbackForce;
            KnockbackHeight = hit.KnockbackHeight;
            ImpactVelocity  = hit.ImpactVelocity;
            Force           = force;
        }

        public HitData WithForce(Vector3 force)
            => new(this, force);
    }

    public readonly struct OnHitEvent
    {
        public EntityId CharacterID { get; }

        public OnHitEvent(EntityId characterID)
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

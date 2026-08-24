using System.Collections.Generic;
using Game.Core.Entities;
using Game.Core.GameEvents;

namespace Game.Core.Systems
{
    internal sealed class HitDetectionState
    {
        private const int CExpectedTargetsPerAttack = 16;

        public readonly HashSet<EntityId> HitTargets = new(CExpectedTargetsPerAttack);

        public bool  IsActive          { get; set; }
        public AttackType AttackType    { get; private set; }
        public float Elapsed           { get; set; }
        public float ActiveWindowStart { get; set; }
        public float ActiveWindowEnd   { get; set; }
        public float HitboxRange       { get; set; }
        public float HitboxRadius      { get; set; }
        public float HitboxHeight      { get; private set; }
        public float KnockbackForce    { get; private set; }
        public float KnockbackHeight   { get; private set; }

        public void BeginAttack(
            float activeWindowStart,
            float activeWindowEnd,
            float hitboxRange,
            float hitboxRadius,
            float knockbackForce,
            float knockbackHeight
        )
        {
            Reset();
            IsActive          = true;
            AttackType        = AttackType.Simple;
            Elapsed           = 0f;
            ActiveWindowStart = activeWindowStart;
            ActiveWindowEnd   = activeWindowEnd;
            HitboxRange       = hitboxRange;
            HitboxRadius      = hitboxRadius;
            KnockbackForce    = knockbackForce;
            KnockbackHeight   = knockbackHeight;
        }

        public void BeginPowerAttack(
            float activeWindowStart,
            float hitboxRadius,
            float hitboxHeight,
            float knockbackForce,
            float knockbackHeight
        )
        {
            Reset();
            IsActive          = true;
            AttackType        = AttackType.Power;
            ActiveWindowStart = activeWindowStart;
            HitboxRadius      = hitboxRadius;
            HitboxHeight      = hitboxHeight;
            KnockbackForce    = knockbackForce;
            KnockbackHeight   = knockbackHeight;
        }

        public void Reset()
        {
            IsActive          = false;
            Elapsed           = 0f;
            ActiveWindowStart = 0f;
            ActiveWindowEnd   = 0f;
            HitboxRange       = 0f;
            HitboxRadius      = 0f;
            HitboxHeight      = 0f;
            KnockbackForce    = 0f;
            KnockbackHeight   = 0f;
            HitTargets.Clear();
        }
    }
}

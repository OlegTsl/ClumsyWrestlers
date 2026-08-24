namespace Game.Core.Level.Entities
{
    public readonly struct LevelEntityImpactSettings
    {
        public float HitForceMultiplier { get; }
        public float TargetImpactForce { get; }
        public float KnockbackHeight { get; }

        public LevelEntityImpactSettings(
            float hitForceMultiplier,
            float targetImpactForce,
            float knockbackHeight
        )
        {
            HitForceMultiplier = hitForceMultiplier;
            TargetImpactForce = targetImpactForce;
            KnockbackHeight = knockbackHeight;
        }
    }
}

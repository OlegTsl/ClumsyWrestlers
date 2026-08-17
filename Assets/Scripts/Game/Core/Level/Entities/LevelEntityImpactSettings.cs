namespace Game.Core.Level.Entities
{
    public readonly struct LevelEntityImpactSettings
    {
        public float HitForceMultiplier { get; }
        public float TargetImpactForce { get; }

        public LevelEntityImpactSettings(float hitForceMultiplier, float targetImpactForce)
        {
            HitForceMultiplier = hitForceMultiplier;
            TargetImpactForce = targetImpactForce;
        }
    }
}

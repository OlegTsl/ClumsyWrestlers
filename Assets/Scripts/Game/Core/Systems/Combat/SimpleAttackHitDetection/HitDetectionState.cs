namespace Game.Core.Systems
{
    internal sealed class HitDetectionState
    {
        public bool  IsActive          { get; set; }
        public float Elapsed           { get; set; }
        public float ActiveWindowStart { get; set; }
        public float ActiveWindowEnd   { get; set; }
        public float HitboxRange       { get; set; }
        public float HitboxRadius      { get; set; }

        public void BeginAttack(
            float activeWindowStart,
            float activeWindowEnd,
            float hitboxRange,
            float hitboxRadius
        )
        {
            IsActive          = true;
            Elapsed           = 0f;
            ActiveWindowStart = activeWindowStart;
            ActiveWindowEnd   = activeWindowEnd;
            HitboxRange       = hitboxRange;
            HitboxRadius      = hitboxRadius;
        }

        public void Reset()
        {
            IsActive          = false;
            Elapsed           = 0f;
            ActiveWindowStart = 0f;
            ActiveWindowEnd   = 0f;
            HitboxRange       = 0f;
            HitboxRadius      = 0f;
        }
    }
}

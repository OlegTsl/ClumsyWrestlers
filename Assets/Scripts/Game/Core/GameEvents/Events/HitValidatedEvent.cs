namespace Game.Core.GameEvents
{
    public readonly struct OnHitValidatedEvent
    {
        public HitData Hit { get; }

        public OnHitValidatedEvent(HitData hit)
            => Hit = hit;
    }
}

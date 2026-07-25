namespace Game.Core.Events
{
    public readonly struct OnHitBoxEnabledEvent
    {
        public bool Enabled { get; }
        public OnHitBoxEnabledEvent(bool enabled)
            => Enabled = enabled;
    }
}
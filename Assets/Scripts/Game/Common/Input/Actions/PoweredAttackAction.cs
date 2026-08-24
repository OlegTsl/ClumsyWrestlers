namespace Game.Common.Input
{
    public readonly struct PowerAttackAction
    {
        public InputEventType EventType { get; }
        public PowerAttackAction(InputEventType eventType)
            => EventType = eventType;
    }
}

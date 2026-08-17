namespace Game.Common.Input
{
    public readonly struct DashAction
    {
        public InputEventType EventType { get; }
        public DashAction(InputEventType eventType)
            => EventType = eventType;

    }
}

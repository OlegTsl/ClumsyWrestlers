namespace Game.Common.Input
{
    public readonly struct BlockAction
    {
        public InputEventType EventType { get; }
        public BlockAction(InputEventType eventType)
            => EventType = eventType;
    }
}

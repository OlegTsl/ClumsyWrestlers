namespace Game.Common.Input
{
    public readonly struct JumpAction
    {
        public InputEventType EventType { get; }
        public JumpAction(InputEventType eventType)
            => EventType = eventType;
        
    }
}

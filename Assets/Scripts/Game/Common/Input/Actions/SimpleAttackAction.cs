namespace Game.Common.Input
{
    public readonly struct SimpleAttackAction
    {
        public InputEventType EventType { get; }
        
        public SimpleAttackAction(InputEventType eventType)
            => EventType = eventType;
        
    }
}

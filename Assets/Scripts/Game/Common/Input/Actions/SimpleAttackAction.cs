namespace Game.Common.Input
{
    public readonly struct SimpleAttackAction : IActionInput
    {
        public InputEventType EventType { get; }
        
        public SimpleAttackAction(InputEventType eventType)
            => EventType = eventType;
        
        public void Publish(InputEventBus eventBus)
            => eventBus.Publish(this);
    }
}
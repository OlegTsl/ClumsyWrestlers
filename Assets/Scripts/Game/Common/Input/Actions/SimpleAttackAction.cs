namespace Game.Common.Input
{
    public readonly struct SimpleAttackAction : IActionInput
    {
        public InputEventType EventType { get; }
        
        public SimpleAttackAction(InputEventType eventType)
            => EventType = eventType;
        
        public void Publish(InputEventsBus eventBus)
            => eventBus.Publish(this);
    }
}
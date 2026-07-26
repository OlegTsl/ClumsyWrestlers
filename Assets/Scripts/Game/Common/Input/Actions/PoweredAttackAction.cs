namespace Game.Common.Input
{
    public readonly struct PowerAttackAction : IActionInput
    {
        public InputEventType EventType { get; }
        
        public PowerAttackAction(InputEventType eventType)
            => EventType = eventType;
        
        public void Publish(InputEventsBus eventBus)
            => eventBus.Publish(this);
    }
}
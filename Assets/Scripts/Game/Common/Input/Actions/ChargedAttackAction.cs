namespace Game.Common.Input
{
    public readonly struct ChargedAttackAction : IActionInput
    {
        public InputEventType EventType { get; }
        
        public ChargedAttackAction(InputEventType eventType) => EventType = eventType;
        
        public void Publish(InputEventBus eventBus)
            => eventBus.Publish(this);
    }
}
namespace Game.Common.Input
{
    public readonly struct JumpAction : IActionInput
    {
        public InputEventType EventType { get; }
        public JumpAction(InputEventType eventType)
            => EventType = eventType;
        
        public void Publish(InputEventsBus eventBus)
            => eventBus.Publish(this);
    }
}
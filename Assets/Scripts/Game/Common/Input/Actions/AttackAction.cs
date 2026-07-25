namespace Game.Common.Input
{
    public readonly struct AttackAction : IActionInput
    {
        public InputEventType EventType { get; }
        public AttackAction(InputEventType eventType)
            => EventType = eventType;

        public void Publish(InputEventsBus eventBus)
            => eventBus.Publish(this);
    }
}
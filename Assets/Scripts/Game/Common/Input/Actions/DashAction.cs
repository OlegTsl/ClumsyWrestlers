namespace Game.Common.Input
{
    public readonly struct DashAction : IActionInput
    {
        public InputEventType EventType { get; }
        public DashAction(InputEventType eventType)
            => EventType = eventType;

        public void Publish(InputEventsBus eventBus)
            => eventBus.Publish(this);
    }
}
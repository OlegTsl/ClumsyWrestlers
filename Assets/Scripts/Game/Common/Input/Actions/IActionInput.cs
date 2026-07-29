namespace Game.Common.Input
{
    public interface IActionInput
    {
        InputEventType EventType { get; }
        void Publish(InputEventsBus eventBus);
    }
}

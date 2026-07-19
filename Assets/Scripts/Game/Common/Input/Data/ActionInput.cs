namespace Game.Common.Input
{
    public readonly struct ActionInput
    {
        public string ActionId          { get; }
        public InputEventType EventType { get; }

        public ActionInput(string actionId, InputEventType eventType)
        {
            ActionId  = actionId;
            EventType = eventType;
        }
    }
}
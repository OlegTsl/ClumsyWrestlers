namespace Game.Common.Input
{
    public interface IInputSource
    {
        int Priority  { get; }
        bool IsActive { get; }
        
        MoveInput GetMoveInput();
        LookInput GetLookInput();
        
        void PublishActions(IInputEventsBus eventBus);
    }
}

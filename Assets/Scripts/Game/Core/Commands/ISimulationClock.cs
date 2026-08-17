namespace Game.Core.Commands
{
    public interface ISimulationClock
    {
        uint CurrentTick { get; }
        uint NextTick { get; }
        uint Advance();
        void Reset();
    }
}

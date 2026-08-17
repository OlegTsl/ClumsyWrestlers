namespace Game.Core.Commands
{
    public sealed class SimulationClock : ISimulationClock
    {
        public uint CurrentTick { get; private set; }
        public uint NextTick => CurrentTick + 1u;

        public uint Advance()
            => ++CurrentTick;

        public void Reset()
            => CurrentTick = 0u;
    }
}

using Game.Core.Commands;
using Game.Core.Entities;

namespace Game.Core.Bots
{
    public interface IBotDecisionAgent
    {
        EntityId CharacterId { get; }
        void CollectCommands(uint simulationTick, ICharacterCommandSink commandSink);
    }
}

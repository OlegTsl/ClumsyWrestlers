using Game.Core.Character;
using Game.Core.Level;

namespace Game.Core.Bots
{
    public interface IBotDecisionAgentFactory
    {
        IBotDecisionAgent Create(
            ICharacterModel character,
            IBotNavigationAgent navigation,
            ILevelEntityRegistry levelEntities,
            ILevelArenaData arena,
            IBotBehaviorSettings settings);
    }
}

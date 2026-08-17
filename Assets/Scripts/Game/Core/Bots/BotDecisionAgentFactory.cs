using Game.Core.Character;
using Game.Core.Level;
using Game.Core.Teams;

namespace Game.Core.Bots
{
    public sealed class BotDecisionAgentFactory : IBotDecisionAgentFactory
    {
        private readonly ICharacterContext _characters;
        private readonly ITeamRelations _teamRelations;

        public BotDecisionAgentFactory(
            ICharacterContext characters,
            ITeamRelations teamRelations)
        {
            _characters = characters;
            _teamRelations = teamRelations;
        }

        public IBotDecisionAgent Create(
            ICharacterModel character,
            IBotNavigationAgent navigation,
            ILevelEntityRegistry levelEntities,
            ILevelArenaData arena,
            IBotBehaviorSettings settings)
        {
            IBotPerception perception = new BotPerception(
                character,
                _characters,
                levelEntities,
                arena,
                _teamRelations,
                navigation,
                settings);
            IBotUtilityEvaluator utility = new BotUtilityEvaluator(settings);
            return new BotDecisionAgent(
                character,
                perception,
                utility,
                navigation,
                settings);
        }
    }
}

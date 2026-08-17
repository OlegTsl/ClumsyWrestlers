using Game.Core.Character;
using Game.Core.Extension;
using Game.Core.GameEvents;
using Game.Core.Teams;
using Zenject;

namespace Game.Core.Systems
{
    public sealed class TeamVictorySystem : ITeamVictorySystem, IFixedTickable
    {
        private readonly IGameEventsBus    _events;
        private readonly ICharacterContext _characters;

        private bool _hasWinner;
        private bool _isEvaluationRequested;

        public TeamVictorySystem(IGameEventsBus events, ICharacterContext characters)
        {
            _events     = events;
            _characters = characters;
            _events.Subscribe<OnCharacterEliminatedEvent>(OnCharacterEliminated);
        }

        private void OnCharacterEliminated(OnCharacterEliminatedEvent evt)
            => _isEvaluationRequested = true;

        public void FixedTick()
        {
            if (!_isEvaluationRequested || _hasWinner)
                return;

            _isEvaluationRequested = false;
            if (!TryGetSoleActiveTeam(out TeamId winningTeam))
                return;

            _hasWinner = true;
            _events.Publish(new OnTeamWonEvent(winningTeam));
        }

        private bool TryGetSoleActiveTeam(out TeamId teamId)
        {
            teamId = TeamId.Invalid;
            
            var characters = _characters.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                ICharacterModel character = characters[i];
                if (!character.GetState<ICharacterActivityState>().Enabled)
                    continue;

                TeamId candidate = character.GetState<ICharacterTeamState>().TeamId;
                if (!teamId.IsValid)
                    teamId = candidate;
                else if (teamId != candidate)
                    return false;
            }

            return teamId.IsValid;
        }

        public void Dispose()
            => _events.Unsubscribe<OnCharacterEliminatedEvent>(OnCharacterEliminated);
    }
}

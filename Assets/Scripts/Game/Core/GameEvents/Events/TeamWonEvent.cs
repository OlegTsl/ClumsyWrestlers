using Game.Core.Teams;

namespace Game.Core.GameEvents
{
    public readonly struct OnTeamWonEvent
    {
        public TeamId TeamId { get; }

        public OnTeamWonEvent(TeamId teamId)
            => TeamId = teamId;
    }
}

using Game.Core.Bots;
using Game.Core.Teams;

namespace Game.Core.Round
{
    public interface IRoundConfiguration
    {
        string CharacterAddress { get; }
        TeamId PlayerTeamId { get; }
        TeamId OpponentTeamId { get; }
        int AlliedBotCount { get; }
        int OpponentBotCount { get; }
        IBotBehaviorSettings BotBehaviorSettings { get; }
    }
}

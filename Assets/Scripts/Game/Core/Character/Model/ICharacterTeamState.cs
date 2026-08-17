using Game.Core.Teams;

namespace Game.Core.Character
{
    public interface ICharacterTeamState : ICharacterModel
    {
        TeamId TeamId { get; }
    }
}

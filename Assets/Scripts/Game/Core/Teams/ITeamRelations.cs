namespace Game.Core.Teams
{
    public interface ITeamRelations
    {
        bool AreAllies(TeamId first, TeamId second);
        bool AreEnemies(TeamId first, TeamId second);
    }
}

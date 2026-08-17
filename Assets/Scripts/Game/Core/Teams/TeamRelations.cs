namespace Game.Core.Teams
{
    public sealed class TeamRelations : ITeamRelations
    {
        public bool AreAllies(TeamId first, TeamId second)
            => first.IsValid && second.IsValid && first == second;

        public bool AreEnemies(TeamId first, TeamId second)
            => first.IsValid && second.IsValid && first != second;
    }
}

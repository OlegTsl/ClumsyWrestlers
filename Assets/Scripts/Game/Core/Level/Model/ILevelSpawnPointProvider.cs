namespace Game.Core.Level
{
    public interface ILevelSpawnPointProvider
    {
        LevelSpawnPoint GetCharacterSpawnPoint(
            bool isPlayerTeam,
            int spawnIndex);
    }
}

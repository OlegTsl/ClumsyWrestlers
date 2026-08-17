namespace Game.Core.Level
{
    public interface ILevelSpawnPointProvider
    {
        LevelSpawnPoint GetCharacterSpawnPoint(bool isPlayer);
    }
}

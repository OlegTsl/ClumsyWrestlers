using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core.Character;

namespace Game.Core.Level
{
    public interface ILevelController : IDisposable
    {
        ILevelModel Level  { get; }
        bool IsLevelLoaded { get; }
        
        void UnloadLevel();
        UniTask LoadLevelAsync(string address, CancellationToken cancellationToken);
        void SpawnCharacter(ICharacterModel model, bool isPlayerTeam, int spawnIndex);
    }
}

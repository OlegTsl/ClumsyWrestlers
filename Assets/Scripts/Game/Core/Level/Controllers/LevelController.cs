using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Common.AssetsManager;
using Game.Core.Character;
using Game.Core.Extension;

namespace Game.Core.Level
{
    public sealed class LevelController : ILevelController
    {
        private readonly IAssetManager         _assetManager;
        private readonly ICharacterViewContext _viewContext;

        private IViewLease<LevelView>    _levelLease;
        private ILevelSpawnPointProvider _spawnPointProvider;

        public ILevelModel Level { get; private set; }
        public bool IsLevelLoaded => Level != null;

        public LevelController(
            IAssetManager         assetManager,
            ICharacterViewContext viewContext
        )
        {
            _assetManager = assetManager;
            _viewContext  = viewContext;
        }

        public async UniTask LoadLevelAsync(string address, CancellationToken cancellationToken)
        {
            UnloadLevel();
            IViewLease<LevelView> nextLease = null;

            try
            {
                nextLease = await _assetManager.InstantiateViewAsync<LevelView>(
                    address, null, cancellationToken);
                
                cancellationToken.ThrowIfCancellationRequested();

                LevelModel level = new(nextLease.View);
                _levelLease = nextLease;
                
                Level = level;

                _spawnPointProvider = level;
                nextLease = null;
            }
            finally
            {
                nextLease?.Dispose();
            }
        }

        public void SpawnCharacter(ICharacterModel model, bool isPlayerTeam, int spawnIndex)
        {
            if (Level == null)
            {
                throw new InvalidOperationException(
                    "A level must be loaded before spawning.");
            }

            ICharacterView view = _viewContext.GetView(model.CharacterID);
            if (view == null)
            {
                throw new InvalidOperationException(
                    $"Character {model.CharacterID} has no registered view.");
            }

            LevelSpawnPoint spawnPoint = _spawnPointProvider.GetCharacterSpawnPoint(
                isPlayerTeam, spawnIndex);

            ICharacterTransformState transform = model.GetState<ICharacterTransformState>();
            ICharacterPhysicsState   physics   = model.GetState<ICharacterPhysicsState>();
            ICharacterActivityState  activity  = model.GetState<ICharacterActivityState>();

            view.Show();
            view.SetPosition(spawnPoint.Position);
            view.SetRotation(spawnPoint.Rotation);

            transform.SetPosition(spawnPoint.Position);
            transform.SetRotation(spawnPoint.Rotation);

            physics.SetVelocity(UnityEngine.Vector3.zero);
            activity.SetEnabled(true);
        }

        public void UnloadLevel()
        {
            Level?.Dispose();
            Level = null;

            _spawnPointProvider = null;

            _levelLease?.Dispose();
            _levelLease = null;
        }

        public void Dispose()
            => UnloadLevel();
    }
}

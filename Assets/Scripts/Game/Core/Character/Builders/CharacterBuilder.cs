using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Common.AssetsManager;
using Game.Core.Entities;
using Game.Core.Extension;
using Game.Core.Teams;

namespace Game.Core.Character
{
    public sealed class CharacterBuilder : ICharacterBuilder
    {
        private readonly IAssetManager _assetManager;
        private readonly IEntityIdAllocator _entityIdAllocator;
        private readonly ICharacterContext _characterContext;
        private readonly ICharacterViewContext _viewContext;

        public CharacterBuilder(
            IAssetManager assetManager,
            IEntityIdAllocator entityIdAllocator,
            ICharacterContext characterContext,
            ICharacterViewContext viewContext
        )
        {
            _assetManager = assetManager;
            _entityIdAllocator = entityIdAllocator;
            _characterContext = characterContext;
            _viewContext = viewContext;
        }

        public UniTask<ICharacterRuntime> BuildCharacterAsync(
            string address,
            TeamId teamId,
            CancellationToken cancellationToken
        )
            => BuildCharacterAsync(
                address,
                _entityIdAllocator.Allocate(),
                teamId,
                cancellationToken);

        public async UniTask<ICharacterRuntime> BuildCharacterAsync(
            string address,
            EntityId characterId,
            TeamId teamId,
            CancellationToken cancellationToken
        )
        {
            if (!characterId.IsValid)
            {
                throw new System.ArgumentException(
                    "Character id must be valid.",
                    nameof(characterId));
            }

            if (!teamId.IsValid)
            {
                throw new System.ArgumentException(
                    "Team id must be valid.",
                    nameof(teamId));
            }

            IViewLease<CharacterView> viewLease = null;
            try
            {
                viewLease = await _assetManager.InstantiateViewAsync<CharacterView>(
                    address,
                    null,
                    cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                CharacterView view = viewLease.View;
                view.Hide();

                CharacterModel model = new(characterId, view.Data, teamId);
                model.GetState<ICharacterPhysicsState>().SynchronizePhysics(view.CapturePhysicsSnapshot());

                CharacterRuntime runtime = new(
                    model,
                    viewLease,
                    _characterContext,
                    _viewContext);
                viewLease = null;
                return runtime;
            }
            finally
            {
                viewLease?.Dispose();
            }
        }
    }
}

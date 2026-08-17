using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core.Entities;
using Game.Core.Teams;

namespace Game.Core.Character
{
    public interface ICharacterBuilder
    {
        UniTask<ICharacterRuntime> BuildCharacterAsync(
            string address,
            TeamId teamId,
            CancellationToken cancellationToken);

        UniTask<ICharacterRuntime> BuildCharacterAsync(
            string address,
            EntityId characterId,
            TeamId teamId,
            CancellationToken cancellationToken);
    }
}

using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core.Entities;

namespace Game.Core.Character
{
    public interface ICharacterBuilder
    {
        UniTask<ICharacterRuntime> BuildCharacterAsync(
            string address,
            CancellationToken cancellationToken);

        UniTask<ICharacterRuntime> BuildCharacterAsync(
            string address,
            EntityId characterId,
            CancellationToken cancellationToken);
    }
}

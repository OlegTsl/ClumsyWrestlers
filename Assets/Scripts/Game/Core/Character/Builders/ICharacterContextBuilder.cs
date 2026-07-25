using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Core.Character
{
    public interface ICharacterContextBuilder
    {
        UniTask<ICharacterContext> BuildPlayerContext(
            string     name,
            Vector3    position,
            Quaternion rotation);
    }
}
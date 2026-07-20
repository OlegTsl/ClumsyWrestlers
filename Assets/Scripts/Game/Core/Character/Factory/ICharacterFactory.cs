using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Core.Character
{
    public interface ICharacterFactory
    {
        UniTask<ICharacterView> Create(string name, Vector3 position, Quaternion rotation);
    }
}
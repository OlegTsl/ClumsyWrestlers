using UnityEngine;

namespace Game.Core.Character
{
    public interface ICharacterRegistry
    {
        void Register(ICharacterView view);
        void Unregister(ICharacterView view);
        ICharacterView GetByCollider(Collider collider);
    }
}
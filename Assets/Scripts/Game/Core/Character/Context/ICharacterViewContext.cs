using Game.Core.Entities;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Character
{
    public interface ICharacterViewContext
    {
        void AddView(EntityId characterId, ICharacterView view);
        void RemoveView(EntityId characterId);
        ICharacterView GetView(EntityId characterId);
        bool TryGetCharacterId(Collider collider, out EntityId characterId);
    }
}

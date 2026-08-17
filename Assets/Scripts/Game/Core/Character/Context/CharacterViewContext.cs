using System.Collections.Generic;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Character
{
    public sealed class CharacterViewContext : ICharacterViewContext
    {
        private readonly Dictionary<EntityId, ICharacterView> _views        = new();
        private readonly Dictionary<Collider, EntityId>       _colliderIds  = new();
        private readonly Dictionary<Rigidbody, EntityId>      _rigidbodyIds = new();

        public void AddView(EntityId characterId, ICharacterView view)
        {
            _views.Add(characterId, view);
            _colliderIds.Add(view.Hitbox, characterId);
            _rigidbodyIds.Add(view.Rigidbody, characterId);
        }

        public void RemoveView(EntityId characterId)
        {
            if (!_views.TryGetValue(characterId, out ICharacterView view))
                return;

            _views.Remove(characterId);
            _colliderIds.Remove(view.Hitbox);
            _rigidbodyIds.Remove(view.Rigidbody);
        }

        public ICharacterView GetView(EntityId characterId)
        {
            _views.TryGetValue(characterId, out ICharacterView view);
            return view;
        }

        public bool TryGetCharacterId(Collider collider, out EntityId characterId)
        {
            if (_colliderIds.TryGetValue(collider, out characterId))
                return true;

            Rigidbody attachedRigidbody = collider.attachedRigidbody;
            return attachedRigidbody != null && _rigidbodyIds.TryGetValue(attachedRigidbody, out characterId);
        }
    }
}

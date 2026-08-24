using System.Collections.Generic;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Character
{
    public sealed class CharacterViewContext : ICharacterViewContext
    {
        private readonly Dictionary<EntityId, ViewRegistration> _views        = new();
        private readonly Dictionary<Collider, EntityId>         _colliderIds  = new();
        private readonly Dictionary<Rigidbody, EntityId>        _rigidbodyIds = new();

        public void AddView(EntityId characterId, ICharacterView view)
        {
            var registration = new ViewRegistration(view);
            _views.Add(characterId, registration);
            _colliderIds.Add(registration.Hitbox, characterId);
            _rigidbodyIds.Add(registration.Rigidbody, characterId);
        }

        public void RemoveView(EntityId characterId)
        {
            if (!_views.Remove(characterId, out ViewRegistration registration))
                return;

            _colliderIds.Remove(registration.Hitbox);
            _rigidbodyIds.Remove(registration.Rigidbody);
        }

        public ICharacterView GetView(EntityId characterId)
        {
            return _views.TryGetValue(characterId, out ViewRegistration registration) &&
                   registration.View.IsAlive
                ? registration.View
                : null;
        }

        public bool TryGetCharacterId(Collider collider, out EntityId characterId)
        {
            if (_colliderIds.TryGetValue(collider, out characterId))
                return true;

            Rigidbody attachedRigidbody = collider.attachedRigidbody;
            return attachedRigidbody != null && _rigidbodyIds.TryGetValue(attachedRigidbody, out characterId);
        }

        private readonly struct ViewRegistration
        {
            public readonly ICharacterView View;
            public readonly Collider       Hitbox;
            public readonly Rigidbody      Rigidbody;

            public ViewRegistration(ICharacterView view)
            {
                View      = view;
                Hitbox    = view.Hitbox;
                Rigidbody = view.Rigidbody;
            }
        }
    }
}

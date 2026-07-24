using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Character
{
    public class CharacterRegistry : ICharacterRegistry
    {
        private readonly Dictionary<Collider, ICharacterView> _hitBoxes = new();
        private readonly List<ICharacterView> _allCharacters = new();

        public void Register(ICharacterView view)
        {
            _allCharacters.Add(view);
            _hitBoxes[view.HitBox] = view;
        }

        public void Unregister(ICharacterView view)
        {
            _allCharacters.Remove(view);
            _hitBoxes.Remove(view.HitBox);
        }

        public ICharacterView GetByCollider(Collider collider)
        {
            _hitBoxes.TryGetValue(collider, out var view);
            return view;
        }
    }
}
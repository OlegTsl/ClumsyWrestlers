using System;
using Cysharp.Threading.Tasks;
using Game.Common.AssetsManager;
using UnityEngine;

namespace Game.Core.Character
{
    public class CharacterBuilder : ICharacterBuilder
    {
        private readonly IAssetManager     _assetManager;
        private readonly ICharacterContext _context;

        public CharacterBuilder(
            IAssetManager     assetManager,
            ICharacterContext context
        )
        {
            _assetManager = assetManager;
            _context      = context;
        }

        public async UniTask<ICharacterModel> BuidCharacter(string name)
        {
            var view = await LoadView(name);
            if (view == null)
                return null;

            var character = new CharacterModel(view, Guid.NewGuid());
            character.SetEnabled(false);

            _context.AddCharacter(character);
            
            return character;
        }

        private async UniTask<ICharacterView> LoadView(string name)
        {
            var view = await _assetManager.LoadView<CharacterView>(name, null);
            if (view == null)
            {
                Debug.LogError("Failed to load character");
                return null;
            }
            return view;
        } 
    }
}
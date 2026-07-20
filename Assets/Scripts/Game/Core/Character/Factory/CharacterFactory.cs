using Cysharp.Threading.Tasks;
using Game.Common.AssetsManager;
using UnityEngine;

namespace Game.Core.Character
{
    public class CharacterFactory : ICharacterFactory
    {
        private readonly IAssetManager _assetManager;

        public CharacterFactory(IAssetManager assetManager)
            => _assetManager = assetManager;

        public async UniTask<ICharacterView> Create(string name, Vector3 position, Quaternion rotation)
        {
            var view = await _assetManager.LoadView<ICharacterView>(name, null);
            if (view == null)
            {
                Debug.LogError("Failed to load character");
                return null;
            }

            view.Transform.position = position;
            view.Transform.rotation = rotation;
            view.Hide();

            return view;
        }
    }
}
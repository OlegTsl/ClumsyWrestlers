using System;
using Cysharp.Threading.Tasks;
using Game.Common.AssetsManager;
using Game.Common.Input;
using Game.Core.Animation;
using Game.Core.Combat;
using Game.Core.Events;
using Game.Core.Movement;
using UnityEngine;

namespace Game.Core.Character
{
    public class CharacterContextBuilder : ICharacterContextBuilder
    {
        private readonly IAssetManager       _assetManager;
        private readonly InputEventBus       _inputEventsBus;
        private readonly ICharactersRegistry _charactersRegistry;

        public CharacterContextBuilder(
            IAssetManager       assetManager,
            InputEventBus       inputEventsBus,
            ICharactersRegistry charactersRegistry
        )
        {
            _assetManager       = assetManager;
            _inputEventsBus     = inputEventsBus;
            _charactersRegistry = charactersRegistry;
        }

        public async UniTask<ICharacterContext> BuildPlayerContext(
            string     name,
            Vector3    position,
            Quaternion rotation
        )
        {
            var view = await LoadView(name);
            if (view == null)
                return null;

            view.Transform.position = position;
            view.Transform.rotation = rotation;

            var gameEventsBus = new GameEventsBus();
            var context       = new CharacterContext
            {
                View        = view,
                Animation   = new AnimationController(view.Transform,      view.Animator,            gameEventsBus                          ),
                Movement    = new MovementController (view.Transform,      view.Rigidbody,           view.Data.Movement                     ),
                Damage      = new DamageController   (view.Transform,      view.Data.AttackSettings, gameEventsBus,      _charactersRegistry),
                Combat      = new CombatController   (view,                view.Data.AttackSettings, _inputEventsBus,    gameEventsBus      ),
                Hit         = new HitController      (_charactersRegistry, gameEventsBus                                                    ),
                GameEvents  = gameEventsBus,
                CharacterId = Guid.NewGuid()
            };

            view.ColliderHandler.Initialize(context.Hit);
            return context;
        }

        private async UniTask<ICharacterView> LoadView(string name)
        {
            var view = await _assetManager.LoadView<CharacterView>(name, null);
            if (view == null)
            {
                Debug.LogError("Failed to load character");
                return null;
            }

            view.Hide();
            return view;
        } 
    }
}
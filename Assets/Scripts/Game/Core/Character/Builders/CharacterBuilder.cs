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

        public async UniTask<ICharacter> BuidCharacter(string name)
        {
            var view = await LoadView(name);
            if (view == null)
                return null;

            var character = new Character(view, Guid.NewGuid());
            character.SetEnabled(false);

            _context.AddCharacter(character);
            
            return character;
        }

        /*public async UniTask<ICharacterContext> BuildCharacterContext(
            string                    name,
            bool                      isPlayer,
            Vector3                   position,
            Quaternion                rotation,
            IEnumerable<IInputSource> inputSources
        )
        {
            var view = await LoadView(name);
            if (view == null)
                return null;

            view.Transform.position = position;
            view.Transform.rotation = rotation;

            var gameEventsBus  = new GameEventsBus();
            var inputEventsBus = new InputEventsBus();

            var hitController = new HitController(gameEventsBus, _charactersRegistry);

            var context = new CharacterContext(
                view, Guid.NewGuid(), inputEventsBus, gameEventsBus);

            context.AddSystem(
                new InputController(inputSources, inputEventsBus));

            context.AddSystem<IMovementController>(
                new MovementController(view.Transform, view.Rigidbody, view.Data.Movement));

            context.AddSystem<IAnimationController>(
                new AnimationController(view.Transform, view.Animator, gameEventsBus));

            context.AddSystem<ILookController>(
                new LookController(view.Transform, view.CharacterCamera.transform, inputEventsBus));

            context.AddSystem<ICombatController>(
                new CombatController(view.Data.AttackSettings, gameEventsBus, inputEventsBus));

            context.AddSystem<IDamageController>(
                new DamageController(view.Transform, view.Data.AttackSettings, gameEventsBus, _charactersRegistry));

            context.AddSystem<IHitController>(
                hitController);

            view.ColliderHandler.Initialize(hitController);
            view.SetAsPlayer(isPlayer);

            return context;
        }*/

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
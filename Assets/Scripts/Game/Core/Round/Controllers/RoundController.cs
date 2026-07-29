using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Common.AssetsManager;
using Game.Common.Input;
using Game.Core.Character;
using Game.Core.GameEvents;
using Game.Core.Level;

namespace Game.Core.Round
{
    public class RoundController : IRoundController
    {
        private readonly ILevelController  _levelController;
        private readonly ICharacterBuilder _characterBuilder;
        private readonly IAssetManager     _assetManager;
        private readonly GameEventsBus     _gameEventsBus;
        private readonly InputEventsBus    _inputEventsBus;
        
        private List<ICharacterController> _characterControllers = new(); 

        public RoundController(
            ILevelController  levelController,
            ICharacterBuilder characterBuilder,
            IAssetManager     assetManager,
            GameEventsBus     gameEventsBus,
            InputEventsBus    inputEventsBus
        )
        {
            _levelController  = levelController;
            _characterBuilder = characterBuilder;
            _assetManager     = assetManager;
            _gameEventsBus    = gameEventsBus;
            _inputEventsBus   = inputEventsBus;
        }

        public async UniTask StartRound(string levelAddress)
        {
            await LoadLevel(levelAddress);

            var (player, enemy) = await UniTask.WhenAll(
                _characterBuilder.BuidCharacter("Wrestler"),
                _characterBuilder.BuidCharacter("Wrestler")
            );

            if (player != null)
            {
                var inputSources = new IInputSource[]
                {
                    new KeyboardSource(priority: 0),
                    new MouseSource   (priority: 1)
                };

                _characterControllers.Add(new CharacterController(
                    player, inputSources, _inputEventsBus, _gameEventsBus));
                _levelController.SpawnCharacter(player, true);
            }

            if (enemy != null)
            {
                var inputSources = new IInputSource[] { };
                _characterControllers.Add(new CharacterController(
                    enemy, inputSources, _inputEventsBus, _gameEventsBus));
                _levelController.SpawnCharacter(enemy, false);
            }
        }

        private UniTask LoadLevel(string name)
            => _levelController.LoadLevel(name);

        private void UnloadLevel()
            => _levelController.UnloadLevel();

        private void UnloadCharacter(string name)
            => _assetManager.UnloadAsset(name);

        public void EndRound()
        {
            UnloadLevel();
            UnloadCharacter("Wrestler");

            foreach (var controller in _characterControllers)
            {
                controller.Dispose();
            }
            _characterControllers.Clear();
        }
    }
}
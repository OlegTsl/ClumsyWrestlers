using Cysharp.Threading.Tasks;
using Game.Common.AssetsManager;
using Game.Common.Input;
using Game.Core.Character;
using Game.Core.GameEvents;
using Game.Core.Level;
using Zenject;

namespace Game.Core.Round
{
    public class RoundController : IRoundController, ILateTickable
    {
        private readonly ICharacterContext  _context;
        private readonly ILevelController   _levelController;
        private readonly ICharacterBuilder  _characterBuilder;
        private readonly IAssetManager      _assetManager;
        private readonly GameEventsBus      _gameEventsBus;
        private readonly InputEventsBus     _inputEventsBus;
        private readonly IRoundSystemsScope _systemsScope;

        private ICharacterController _characterController;
        private IBotController       _botController;
        private ICharacterModel      _player;
        private ICharacterModel      _enemy;
        
        public RoundController(
            ICharacterContext context,
            ILevelController  levelController,
            ICharacterBuilder characterBuilder,
            IAssetManager     assetManager,
            GameEventsBus     gameEventsBus,
            InputEventsBus    inputEventsBus,
            IRoundSystemsScope systemsScope
        )
        {
            _context          = context;
            _levelController  = levelController;
            _characterBuilder = characterBuilder;
            _assetManager     = assetManager;
            _gameEventsBus    = gameEventsBus;
            _inputEventsBus   = inputEventsBus;
            _systemsScope     = systemsScope;
        }

        public async UniTask StartRound(string levelAddress)
        {
            EndRound();

            try
            {
                await LoadLevel(levelAddress);

                var (player, enemy) = await UniTask.WhenAll(
                    _characterBuilder.BuidCharacter("Wrestler"),
                    _characterBuilder.BuidCharacter("Wrestler")
                );

                _player = player;
                _enemy  = enemy;

                if (player != null)
                {
                    var inputSources = new IInputSource[]
                    {
                        new KeyboardSource(priority: 0),
                        new MouseSource   (priority: 1)
                    };

                    _characterController = new CharacterController(
                        player, inputSources, _inputEventsBus, _gameEventsBus);

                    _levelController.SpawnCharacter(player, true);
                }

                if (enemy != null)
                {
                    _botController = new BotController(enemy);
                    _levelController.SpawnCharacter(enemy, false);
                }

                _systemsScope.StartRound();
            }
            catch
            {
                EndRound();
                throw;
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
            if (_characterController != null)
            {
                _characterController.Dispose();
                _characterController = null;
            }

            if (_botController != null)
            {
                _botController.Dispose();
                _botController = null;
            }

            _systemsScope.EndRound();

            DisposeCharacter(ref _player);
            DisposeCharacter(ref _enemy);

            UnloadLevel();
            UnloadCharacter("Wrestler");
        }

        private void DisposeCharacter(ref ICharacterModel character)
        {
            if (character == null)
                return;

            _context.RemoveCharacter(character.CharacterID);
            character.Dispose();
            character = null;
        }

        public void LateTick()
        {
            if (_characterController != null)
                _characterController.LateTick();
        }

        public void Dispose()
            => EndRound();
    }
}

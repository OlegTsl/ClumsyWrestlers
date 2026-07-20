using Cysharp.Threading.Tasks;
using Game.Core.Character;
using Game.Core.Level;

namespace Game.Core.Round
{
    public class RoundController : IRoundController
    {
        private readonly ICharacterFactory    _characterFactory;
        private readonly ICharacterController _characterController;
        private readonly ILevelController     _levelController;

        private ICharacterView _player;
        private ILevelView     _currentLevel;

        public RoundController(
            ICharacterFactory    characterFactory,
            ICharacterController characterController,
            ILevelController     levelController
        )
        {
            _characterFactory    = characterFactory;
            _characterController = characterController;
            _levelController     = levelController;
        }

        public async UniTask StartRound(string levelAddress)
        {
            _currentLevel = await _levelController.LoadLevel(levelAddress);
            if (_currentLevel == null)
                return;

            var spawnPoint = _currentLevel.PlayerSpawnPoint;
            _player = await _characterFactory.Create("Wrestler", spawnPoint.position, spawnPoint.rotation);

            _characterController.Initialize(_player);
            _characterController.Enable();

            _player.Show();
        }

        public void EndRound()
        {
            _characterController.Disable();
            _levelController.UnloadLevel();

            if (_currentLevel != null)
            {
                //Object.Destroy(_currentLevel);
                _currentLevel = null;
            }
        }
    }
}
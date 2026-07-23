using Cysharp.Threading.Tasks;
using Game.Core.Level;

namespace Game.Core.Round
{
    public class RoundController : IRoundController
    {
        private readonly ILevelController _levelController;

        public RoundController(
            ILevelController levelController
        )
        {
            _levelController = levelController;
        }

        public async UniTask StartRound(string levelAddress)
        {
            await _levelController.LoadLevel(levelAddress);

            if (_levelController.IsLevelLoaded())
            {
                await _levelController.SpawnPlayer("Wrestler");
                await _levelController.SpawnEnemy("Wrestler");
            }
        }

        public void EndRound()
        {
            _levelController.UnloadLevel();
        }
    }
}
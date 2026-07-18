using Game.Core.Controllers;

namespace Game.Core
{
    public class GameEntryPointManager
    {
        private readonly IMainGameController _mainGameController;

        public GameEntryPointManager(IMainGameController mainGameController)
            => _mainGameController = mainGameController;

        public void Initialize()
            =>  _mainGameController.RunGame();
    }
}
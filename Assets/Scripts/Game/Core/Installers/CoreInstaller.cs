using Game.Core.Controllers;
using Zenject;

namespace Game.Core.Installers
{
    public class CoreInstaller : Installer<CoreInstaller>
    {
        public override void InstallBindings()
        {
            //Container.BindSingleView<ICanvasView>("Canvas");

            BindSystems();

            Container.BindInterfacesAndSelfTo<MainGameController>().AsSingle();
            Container.Bind<GameEntryPointManager>().AsTransient();
        }

        private void BindSystems()
        {
            /*Container.Bind<BoardInitializeSystem>().AsTransient();
            Container.Bind<UIInitializeSystem>().AsTransient();
            Container.Bind<PlayerInitializeSystem>().AsTransient();
            Container.Bind<StrikerSetupSystem>().AsTransient();
            Container.Bind<StrikerPositioningSystem>().AsTransient();
            Container.Bind<StrikerVisualSystem>().AsTransient();
            Container.Bind<StrikerMovingSystem>().AsTransient();
            Container.Bind<RoundGoalSystem>().AsTransient();
            Container.Bind<PlayerTurnCheckSystem>().AsTransient();
            Container.Bind<PlayerScoreSystem>().AsTransient();
            Container.Bind<PlayerScoreVisualSystem>().AsTransient();
            Container.Bind<PlayerTurnSwitchSystem>().AsTransient();
            Container.Bind<BoardRotationSystem>().AsTransient();
            Container.Bind<RoundStartSystem>().AsTransient();
            Container.Bind<RoundCompleteSystem>().AsTransient();

            Container.Bind<GameSystems>().AsTransient();*/
        }
    }
}
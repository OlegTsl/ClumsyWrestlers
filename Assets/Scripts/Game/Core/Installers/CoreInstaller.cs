using Game.Core.Character;
using Game.Core.Controllers;
using Game.Core.Level;
using Game.Core.Movement;
using Game.Core.Round;
using Zenject;

namespace Game.Core.Installers
{
    public class CoreInstaller : Installer<CoreInstaller>
    {
        public override void InstallBindings()
        {
            //Container.BindSingleView<ICanvasView>("Canvas");

            Container.BindInterfacesAndSelfTo<MainGameController>().AsSingle();
            Container.Bind<GameEntryPointManager>().AsTransient();
            Container.Bind<IRoundController>().To<RoundController>().AsSingle();

            LevelInstaller.Install(Container);
            CharacterInstaller.Install(Container);
            MovementInstaller.Install(Container);
        }
    }
}
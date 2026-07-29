using Game.Core.Character;
using Game.Core.Controllers;
using Game.Core.GameEvents;
using Game.Core.Level;
using Game.Core.Round;
using Game.Core.Systems;
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

            GameEventsInstaller.Install(Container);
            LevelInstaller.Install(Container);
            CharacterInstaller.Install(Container);
            RoundInstaller.Install(Container);
            
            SystemsInstaller.Install(Container);
        }
    }
}
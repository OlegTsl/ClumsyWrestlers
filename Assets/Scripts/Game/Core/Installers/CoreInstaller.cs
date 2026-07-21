using Game.Core.Animation; 
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

            LevelInstaller.Install(Container);
            CharacterInstaller.Install(Container);
            MovementInstaller.Install(Container);
            AnimationInstaller.Install(Container);
            RoundInstaller.Install(Container);
        }
    }
}
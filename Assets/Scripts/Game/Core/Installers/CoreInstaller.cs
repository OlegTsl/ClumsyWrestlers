using Game.Core.Character;
using Game.Core.Controllers;
using Game.Core.GameEvents;
using Game.Core.Level;
using Game.Core.Round;
using Game.Core.Entities;
using Game.Core.Commands;
using Zenject;

namespace Game.Core.Installers
{
    public class CoreInstaller : Installer<CoreInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MainGameController>().AsSingle();
            Container.Bind<GameEntryPointManager>().AsTransient();
            Container.BindInterfacesAndSelfTo<EntityIdAllocator>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterCommandBuffer>().AsSingle();
            Container.BindInterfacesAndSelfTo<SimulationClock>().AsSingle();

            GameEventsInstaller.Install(Container);
            LevelInstaller.Install(Container);
            CharacterInstaller.Install(Container);
            RoundInstaller.Install(Container);
        }
    }
}

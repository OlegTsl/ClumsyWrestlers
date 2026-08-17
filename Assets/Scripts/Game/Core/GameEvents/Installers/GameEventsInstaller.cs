using Zenject;

namespace Game.Core.GameEvents
{
    public class GameEventsInstaller : Installer<GameEventsInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameEventsBus>().AsSingle();
        }
    }
}

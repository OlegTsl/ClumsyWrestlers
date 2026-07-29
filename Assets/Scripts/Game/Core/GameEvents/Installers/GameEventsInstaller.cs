using Zenject;

namespace Game.Core.GameEvents
{
    public class GameEventsInstaller : Installer<GameEventsInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<GameEventsBus>().AsSingle();
        }
    }
}

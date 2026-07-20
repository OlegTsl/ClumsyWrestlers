using Zenject;

namespace Game.Core.Round
{
    public class RoundInstaller : Installer<RoundInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IRoundController>().To<RoundController>().AsSingle();
        }
    }
}

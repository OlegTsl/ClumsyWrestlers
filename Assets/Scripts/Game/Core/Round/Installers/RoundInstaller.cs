using Zenject;

namespace Game.Core.Round
{
    public class RoundInstaller : Installer<RoundInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<RoundSystemsScope>().AsSingle();
            Container.BindInterfacesAndSelfTo<RoundController>().AsSingle().NonLazy();
        }
    }
}

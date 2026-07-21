using Zenject;

namespace Game.Core.Movement
{
    public class MovementInstaller : Installer<MovementInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MovementController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LookController>().AsSingle().NonLazy();
        }
    }
}

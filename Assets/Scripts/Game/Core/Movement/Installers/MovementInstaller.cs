using Zenject;

namespace Game.Core.Movement
{
    public class MovementInstaller : Installer<MovementInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IMovementController>().To<MovementController>().AsTransient();
            Container.BindInterfacesAndSelfTo<LookController>().AsSingle().NonLazy();
        }
    }
}

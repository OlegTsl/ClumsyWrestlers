using Zenject;

namespace Game.Core.Combat
{
    public class CombatInstaller : Installer<CombatInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<ICombatController>().To<CombatController>().AsTransient();
            Container.Bind<IDamageController>().To<DamageController>().AsTransient();
            Container.Bind<IHitController>().To<HitController>().AsTransient();
        }
    }
}

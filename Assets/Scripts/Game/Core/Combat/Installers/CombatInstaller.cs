using Zenject;

namespace Game.Core.Combat
{
    public class CombatInstaller : Installer<CombatInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SimpleAttackController>().AsSingle();
        }
    }
}

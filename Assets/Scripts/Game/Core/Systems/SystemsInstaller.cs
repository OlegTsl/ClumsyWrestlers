using Zenject;

namespace Game.Core.Systems
{
    public class SystemsInstaller : Installer<SystemsInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MovementSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<AnimationSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<AttackSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<CheckDamageSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<ApplyDamageSystem>().AsSingle();
        }
    }
}

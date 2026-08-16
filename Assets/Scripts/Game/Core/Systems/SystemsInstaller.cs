using Zenject;

namespace Game.Core.Systems
{
    public class SystemsInstaller : Installer<SystemsInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MovementSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<AnimationSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<PowerAttackSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<SlamAttackSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<SimpleAttackSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<HitDetectionSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<DamageSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<HitReactionSystem>().AsSingle();
        }
    }
}

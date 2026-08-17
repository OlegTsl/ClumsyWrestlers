using Zenject;

namespace Game.Core.Systems
{
    public class SystemsInstaller : Installer<SystemsInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CharacterPhysicsReadSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<ArenaEliminationSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<TeamVictorySystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterEliminationPresentationSystem>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterCommandDispatchSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterControlStateRegistry>()
                .AsSingle();
            Container.Bind<CharacterMovementCommandPublisher>().AsSingle();
            Container.Bind<ICharacterCommandHandler>()
                .To<MoveCharacterCommandHandler>()
                .AsSingle();
            Container.Bind<ICharacterCommandHandler>()
                .To<JumpCharacterCommandHandler>()
                .AsSingle();
            Container.Bind<ICharacterCommandHandler>()
                .To<SimpleAttackCharacterCommandHandler>()
                .AsSingle();
            Container.Bind<ICharacterCommandHandler>()
                .To<PowerAttackCharacterCommandHandler>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterControlSystem>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MovementSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<AnimationSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<PowerAttackSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<SlamAttackSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<SimpleAttackSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<HitDetectionSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<HitValidationSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<HitResolutionSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<HitReactionSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelEntityTickSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelCollisionSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelImpulseSystem>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<CharacterPhysicsWriteSystem>().AsSingle();

            Container.BindExecutionOrder<CharacterPhysicsReadSystem>(-400);
            Container.BindExecutionOrder<ArenaEliminationSystem>(-375);
            Container.BindExecutionOrder<TeamVictorySystem>(-370);
            Container.BindExecutionOrder<CharacterCommandDispatchSystem>(-300);
            Container.BindExecutionOrder<MovementSystem>(0);
            Container.BindExecutionOrder<SimpleAttackSystem>(10);
            Container.BindExecutionOrder<PowerAttackSystem>(10);
            Container.BindExecutionOrder<LevelEntityTickSystem>(15);
            Container.BindExecutionOrder<HitDetectionSystem>(20);
            Container.BindExecutionOrder<LevelCollisionSystem>(30);
            Container.BindExecutionOrder<CharacterPhysicsWriteSystem>(400);
        }
    }
}

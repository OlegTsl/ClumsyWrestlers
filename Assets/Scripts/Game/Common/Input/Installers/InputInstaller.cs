using Zenject;

namespace Game.Common.Input
{
    public class InputInstaller : Installer<InputInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<InputEventBus>().AsSingle();

            Container.Bind<IInputSource>()
                .To<KeyboardSource>()
                .AsSingle()
                .WithArguments(0);

            Container.Bind<IInputSource>()
                .To<MouseSource>()
                .AsSingle()
                .WithArguments(1);

            Container.Bind<InputController>().AsSingle().NonLazy();
        }
    }
}
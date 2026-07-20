using Zenject;

namespace Game.Core.Character
{
    public class CharacterInstaller : Installer<CharacterInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CharacterController>().AsSingle().NonLazy();
            Container.Bind<ICharacterFactory>().To<CharacterFactory>().AsSingle();
        }
    }
}

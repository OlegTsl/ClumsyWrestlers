using Zenject;

namespace Game.Core.Character
{
    public class CharacterInstaller : Installer<CharacterInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<ICharacterController>().To<CharacterController>().AsSingle();
            Container.Bind<ICharacterFactory>().To<CharacterFactory>().AsSingle();
        }
    }
}

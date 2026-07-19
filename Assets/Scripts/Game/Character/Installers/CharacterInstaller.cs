using Zenject;

namespace Game.Character
{
    public class CharacterInstaller : Installer<CharacterInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<MovementController>().AsSingle();
            Container.Bind<ICharacterController>().To<CharacterController>().AsSingle();
        }
    }
}

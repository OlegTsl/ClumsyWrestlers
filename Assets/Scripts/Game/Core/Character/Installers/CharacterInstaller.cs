using Zenject;

namespace Game.Core.Character
{
    public class CharacterInstaller : Installer<CharacterInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CharactersController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<CharacterContextBuilder>().AsSingle();
            Container.Bind<ICharactersRegistry>().To<CharactersRegistry>().AsSingle();  
        }
    }
}

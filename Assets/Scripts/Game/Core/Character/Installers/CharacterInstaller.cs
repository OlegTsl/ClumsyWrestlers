using Game.Core.Bots;
using Zenject;

namespace Game.Core.Character
{
    public class CharacterInstaller : Installer<CharacterInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CharacterContext>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterViewContext>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterBuilder>().AsSingle();
            Container.BindInterfacesAndSelfTo<BotDecisionAgentFactory>().AsSingle();
        }
    }
}

using Game.Common.Installers;
using Game.Core.Installers;
using Zenject;

namespace Game.Installers
{
    public class MainGameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            CommonInstaller.Install(Container);
            CoreInstaller.Install(Container);
        }
    }
}
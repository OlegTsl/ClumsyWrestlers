using Zenject;
using Game.Common.AssetsManager;
using Game.Common.Localization;
using Game.Common.Input;

namespace Game.Common.Installers
{
    public class CommonInstaller : Installer<CommonInstaller>
    {
        public override void InstallBindings()
        {
            InputInstaller.Install(Container);

            Container.Bind<IAssetManager>().To<AssetManager>().AsSingle();
            Container.Bind<ILocalizationService>().To<LocalizationService>().AsSingle();
        }
    }
}
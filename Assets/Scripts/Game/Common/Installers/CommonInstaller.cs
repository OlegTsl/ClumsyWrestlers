using Zenject;
using Game.Common.AssetsManager;
using Game.Common.Localization;

namespace Game.Common.Installers
{
    public class CommonInstaller : Installer<CommonInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IAssetManager>().To<AssetManager>().AsSingle();
            Container.Bind<ILocalizationService>().To<LocalizationService>().AsSingle();
        }
    }
}
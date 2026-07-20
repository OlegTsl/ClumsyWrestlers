using Zenject;

namespace Game.Core.Level
{
    public class LevelInstaller : Installer<LevelInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle();
        }
    }
}

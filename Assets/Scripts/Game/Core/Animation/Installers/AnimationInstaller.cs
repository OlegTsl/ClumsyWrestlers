using Zenject;

namespace Game.Core.Animation
{
    public class AnimationInstaller : Installer<AnimationInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IAnimationController>().To<AnimationController>().AsTransient();
        }
    }
}

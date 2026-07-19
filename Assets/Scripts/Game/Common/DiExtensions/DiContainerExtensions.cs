using Zenject;
using Cysharp.Threading.Tasks;
using Game.Common.ViewLoader;
using Game.Common.Views;

namespace Game.CommonUtils.DiExtensions
{
    public static class DiContainerExtensions
    {
        public static void BindSingleView<T>(this DiContainer container, string addressableKey) where T : IView
        {
            container.BindInterfacesAndSelfTo<ViewLoader<T>>()
                .AsSingle()
                .WithArguments(addressableKey);

            container.Bind<UniTask<T>>()
                .FromMethod(ctx => ctx.Container.Resolve<IViewLoader<T>>().GetView())
                .AsSingle();
        }
    }
}
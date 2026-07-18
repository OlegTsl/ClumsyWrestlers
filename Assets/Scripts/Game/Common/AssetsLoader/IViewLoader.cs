using Cysharp.Threading.Tasks;
using Game.Common.Views;

namespace Game.Common.AssetsLoader
{
    public interface IViewLoader<T> where T : IView
    {
        UniTask<T> GetView();
        void ResetView();
    }
}
using Cysharp.Threading.Tasks;
using Game.Common.Views;

namespace Game.Common.ViewLoader
{
    public interface IViewLoader<T> where T : IView
    {
        UniTask<T> GetView();
        void ResetView();
    }
}
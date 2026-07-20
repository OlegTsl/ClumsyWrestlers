using Cysharp.Threading.Tasks;

namespace Game.Core.Round
{
    public interface IRoundController
    {
        UniTask StartRound(string levelAddress);
        void EndRound();
    }
}
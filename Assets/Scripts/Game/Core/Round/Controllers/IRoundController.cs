using System;
using Cysharp.Threading.Tasks;

namespace Game.Core.Round
{
    public interface IRoundController : IDisposable
    {
        UniTask StartRound(string levelAddress);
        void EndRound();
    }
}

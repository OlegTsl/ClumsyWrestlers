using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Core.Round
{
    public interface IRoundController : IDisposable
    {
        UniTask StartRoundAsync(string levelAddress, CancellationToken cancellationToken);
        void EndRound();
    }
}

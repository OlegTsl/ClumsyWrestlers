using System;

namespace Game.Core.Round
{
    public interface IRoundSystemsScope : IDisposable
    {
        void StartRound();
        void EndRound();
    }
}

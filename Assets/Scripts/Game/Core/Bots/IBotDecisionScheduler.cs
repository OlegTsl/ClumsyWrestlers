using System;

namespace Game.Core.Bots
{
    public interface IBotDecisionScheduler : IDisposable
    {
        void Register(IBotDecisionAgent agent);
        void Unregister(IBotDecisionAgent agent);
    }
}

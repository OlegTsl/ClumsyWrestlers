using System;

namespace Game.Core.GameEvents
{
    public interface IGameEventsBus
    {
        void Subscribe<T>(Action<T> handler);
        void Unsubscribe<T>(Action<T> handler);
        void Publish<T>(T message);
    }
}

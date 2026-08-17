using System;

namespace Game.Common.Input
{
    public interface IInputEventsBus
    {
        void Subscribe<T>(Action<T> handler);
        void Unsubscribe<T>(Action<T> handler);
        void Publish<T>(T message);
    }
}

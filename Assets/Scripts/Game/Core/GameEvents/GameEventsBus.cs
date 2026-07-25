using System;
using System.Collections.Generic;

namespace Game.Core.Events
{
    public class GameEventsBus
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();
 
        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var existing))
                _handlers[type] = Delegate.Combine(existing, handler);
            else
                _handlers[type] = handler;
        }
 
        public void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var existing))
                return;
 
            var result = Delegate.Remove(existing, handler);
            if (result == null)
                _handlers.Remove(type);
            else
                _handlers[type] = result;
        }
 
        public void Publish<T>(T message)
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var handler))
                ((Action<T>)handler)?.Invoke(message);
        }
    }
}
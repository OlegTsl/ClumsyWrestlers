using System;
using System.Collections.Generic;
using Game.Core.Commands;
using Game.Core.GameEvents;

namespace Game.Core.Systems
{
    public sealed class ControlSystem : IDisposable
    {
        private readonly IGameEventsBus _events;
        private readonly Dictionary<CharacterCommandType, ICommandHandler> _handlers;

        public ControlSystem(
            IGameEventsBus        events,
            List<ICommandHandler> handlers
        )
        {
            _events   = events;
            _handlers = new Dictionary<CharacterCommandType, ICommandHandler>(handlers.Count);

            for (int i = 0; i < handlers.Count; i++)
            {
                ICommandHandler handler = handlers[i];
                if (!_handlers.TryAdd(handler.CommandType, handler))
                {
                    throw new InvalidOperationException(
                        $"Duplicate handler for command {handler.CommandType}.");
                }
            }

            _events.Subscribe<OnCharacterCommandEvent>(OnCommand);
        }

        private void OnCommand(OnCharacterCommandEvent evt)
        {
            CharacterCommand command = evt.Command;
            if (_handlers.TryGetValue(command.Type, out ICommandHandler handler))
                handler.Handle(command);
        }

        public void Dispose()
        {
            _events.Unsubscribe<OnCharacterCommandEvent>(OnCommand);
            _handlers.Clear();
        }
    }
}

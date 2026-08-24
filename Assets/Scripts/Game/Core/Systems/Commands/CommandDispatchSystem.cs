using System;
using Game.Core.Commands;
using Game.Core.GameEvents;
using Zenject;

namespace Game.Core.Systems
{
    public sealed class CommandDispatchSystem : IDisposable, IFixedTickable
    {
        private readonly ICharacterCommandBuffer _commands;
        private readonly ISimulationClock        _clock;
        private readonly IGameEventsBus          _events;

        public CommandDispatchSystem(
            ICharacterCommandBuffer commands,
            ISimulationClock        clock,
            IGameEventsBus          events
        )
        {
            _commands = commands;
            _clock    = clock;
            _events   = events;
        }

        public void FixedTick()
        {
            uint tick = _clock.Advance();
            while (_commands.TryDequeueDue(tick, out CharacterCommand command))
            {
                _events.Publish(new OnCharacterCommandEvent(command));
            }
        }

        public void Dispose()
        {
        }
    }
}

using System.Collections.Generic;
using Game.Core.Commands;
using Zenject;

namespace Game.Core.Bots
{
    public sealed class BotDecisionScheduler : IBotDecisionScheduler, IFixedTickable
    {
        private readonly List<IBotDecisionAgent> _agents = new(16);
        private readonly ICharacterCommandSink _commandSink;
        private readonly ISimulationClock _clock;

        public BotDecisionScheduler(
            ICharacterCommandSink commandSink,
            ISimulationClock clock
        )
        {
            _commandSink = commandSink;
            _clock = clock;
        }

        public void Register(IBotDecisionAgent agent)
        {
            if (!_agents.Contains(agent))
            {
                _agents.Add(agent);
            }
        }

        public void Unregister(IBotDecisionAgent agent)
            => _agents.Remove(agent);

        public void FixedTick()
        {
            uint tick = _clock.NextTick;
            for (int i = 0; i < _agents.Count; i++)
            {
                _agents[i].CollectCommands(tick, _commandSink);
            }
        }

        public void Dispose()
            => _agents.Clear();
    }
}

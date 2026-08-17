using System.Collections.Generic;
using Game.Core.Commands;
using UnityEngine;
using Zenject;

namespace Game.Core.Bots
{
    public sealed class BotDecisionScheduler : IBotDecisionScheduler, IFixedTickable
    {
        private const float DecisionIntervalSeconds = 0.1f;

        private readonly List<IBotDecisionAgent> _agents = new(16);
        private readonly ICharacterCommandSink _commandSink;
        private readonly ISimulationClock _clock;
        private readonly uint _decisionIntervalTicks;

        public BotDecisionScheduler(
            ICharacterCommandSink commandSink,
            ISimulationClock clock
        )
        {
            _commandSink = commandSink;
            _clock = clock;
            _decisionIntervalTicks = (uint)Mathf.Max(
                1,
                Mathf.RoundToInt(DecisionIntervalSeconds / Time.fixedDeltaTime));
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
                if ((tick + (uint)i) % _decisionIntervalTicks == 0u)
                {
                    _agents[i].CollectCommands(tick, _commandSink);
                }
            }
        }

        public void Dispose()
            => _agents.Clear();
    }
}

using System.Collections.Generic;
using System.Linq;

namespace Game.Common.Input
{
    public sealed class InputController
    {
        private readonly List<IInputSource> _sources;
        private readonly InputEventBus      _eventBus;

        public InputController(IEnumerable<IInputSource> sources, InputEventBus eventBus)
        {
            _sources = sources
                .OrderByDescending(s => s.Priority)
                .ToList();
            _eventBus = eventBus;
        }

        public void Tick()
        {
            foreach (var source in _sources)
            {
                if (!source.IsActive)
                    continue;

                var move = source.GetMoveInput();
                if (move.IsActive)
                {
                    _eventBus.Publish(move);
                    break;
                }
            }

            foreach (var source in _sources)
            {
                if (!source.IsActive)
                    continue;

                var look = source.GetLookInput();
                if (look.IsActive)
                {
                    _eventBus.Publish(look);
                    break;
                }
            }

            foreach (var source in _sources)
            {
                if (!source.IsActive)
                    continue;

                var actions = source.GetActionInputs();
                if (actions == null || actions.Count == 0)
                    continue;

                foreach (var action in actions)
                {
                    _eventBus.Publish(action);
                }
            }
        }

        public void RegisterSource(IInputSource source)
        {
            _sources.Add(source);
            _sources.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        public void UnregisterSource(IInputSource source)
            => _sources.Remove(source);
    }
}
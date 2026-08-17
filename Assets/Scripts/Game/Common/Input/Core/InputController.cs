using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Game.Common.Input
{
    public sealed class InputController : ITickable
    {
        private readonly List<IInputSource> _sources;
        private readonly IInputEventsBus _eventBus;

        private bool _wasMoveActive;
        private bool _wasLookActive;

        public InputController(IEnumerable<IInputSource> sources, IInputEventsBus eventBus)
        {
            _sources = sources
                .OrderByDescending(s => s.Priority)
                .ToList();
            _eventBus = eventBus;
        }

        public void Tick()
        {
            PublishMove();
            PublishLook();
            PublishActions();
        }

        private void PublishMove()
        {
            var move = ResolveMove();
            if (move.IsActive || _wasMoveActive)
                _eventBus.Publish(move);
 
            _wasMoveActive = move.IsActive;
        }
 
        private void PublishLook()
        {
            var look = ResolveLook();
            if (look.IsActive || _wasLookActive)
                _eventBus.Publish(look);
 
            _wasLookActive = look.IsActive;
        }
 
        private MoveInput ResolveMove()
        {
            foreach (var source in _sources)
            {
                if (!source.IsActive)
                    continue;
 
                var move = source.GetMoveInput();
                if (move.IsActive)
                    return move;
            }
 
            return MoveInput.None;
        }
 
        private LookInput ResolveLook()
        {
            foreach (var source in _sources)
            {
                if (!source.IsActive)
                    continue;
 
                var look = source.GetLookInput();
                if (look.IsActive)
                    return look;
            }
 
            return LookInput.None;
        }
 
        private void PublishActions()
        {
            for (int i = 0; i < _sources.Count; i++)
            {
                IInputSource source = _sources[i];
                if (!source.IsActive)
                    continue;

                source.PublishActions(_eventBus);
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

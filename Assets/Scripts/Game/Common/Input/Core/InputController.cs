using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Game.Common.Input
{
    public sealed class InputController : ITickable
    {
        private readonly List<IInputSource> _sources;
        private readonly InputEventsBus     _eventBus;

        private bool _wasMoveActive;
        private bool _wasLookActive;

        public InputController(IEnumerable<IInputSource> sources, InputEventsBus eventBus)
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
            foreach (var source in _sources)
            {
                if (!source.IsActive)
                    continue;
                
                var actions = source.GetActionInputs();
                if (actions == null || actions.Count == 0)
                    continue;
                
                foreach (var action in actions)
                {
                    action.Publish(_eventBus);
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
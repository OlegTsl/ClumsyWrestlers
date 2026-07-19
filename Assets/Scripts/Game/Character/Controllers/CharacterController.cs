using Game.Common.Input;

namespace Game.Character
{
    public class CharacterController : ICharacterController
    {
        private readonly InputEventBus      _inputBus;
        private readonly MovementController _movement;
        
        private ICharacterView _view;
        private bool _isEnabled = true;
        private bool _isInitialized;

        public CharacterController(InputEventBus inputBus, MovementController movement)
        {
            _inputBus = inputBus;
            _movement = movement;
        }

        public void Initialize(ICharacterView view)
        {
            _view = view;
            _movement.Initialize(view);
            _isInitialized = true;
            
            _inputBus.Subscribe<MoveInput>(OnMove);
            _inputBus.Subscribe<ActionInput>(OnAction);
        }

        public void FixedTick()
        {
            if (_isEnabled && _isInitialized)
                _movement.FixedTick();
        }

        public void Enable()
            => _isEnabled = true;

        public void Disable()
            => _isEnabled = false;

        private void OnMove(MoveInput input)
        {
            if (_isEnabled)
                _movement.SetMoveDirection(input.Direction);
        }

        private void OnAction(ActionInput action)
        {
            if (!_isEnabled)
                return;
            
            if (action.ActionId == "Jump" && action.EventType == InputEventType.Pressed)
                _movement.Jump();
        }

        public void Dispose()
        {
            _inputBus.Unsubscribe<MoveInput>(OnMove);
            _inputBus.Unsubscribe<ActionInput>(OnAction);
        }
    }
}
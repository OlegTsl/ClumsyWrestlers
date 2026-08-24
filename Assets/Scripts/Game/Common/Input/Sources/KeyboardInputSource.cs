using System.Collections.Generic;
using UnityEngine;

namespace Game.Common.Input
{
    public class KeyboardSource : IInputSource
    {
        public int  Priority { get; }
        public bool IsActive { get; set; } = true;

        private Dictionary<KeyCode, bool> _wasPressed = new();

        public KeyboardSource(int priority = 0)
            => Priority = priority;

        public MoveInput GetMoveInput()
        {
            var direction = Vector3.zero;

            if (UnityEngine.Input.GetKey(KeyCode.W)) direction += Vector3.forward;
            if (UnityEngine.Input.GetKey(KeyCode.S)) direction += Vector3.back;
            if (UnityEngine.Input.GetKey(KeyCode.A)) direction += Vector3.left;
            if (UnityEngine.Input.GetKey(KeyCode.D)) direction += Vector3.right;

            if (direction == Vector3.zero)
                return MoveInput.None;

            return new MoveInput(direction);
        }

        public LookInput GetLookInput()
            => LookInput.None;

        public void PublishActions(IInputEventsBus eventBus)
        {
            PublishJump(eventBus);
            PublishBlock(eventBus);
        }

        private void PublishJump(IInputEventsBus eventBus)
        {
            bool pressed = UnityEngine.Input.GetKey(KeyCode.Space);
            _wasPressed.TryGetValue(KeyCode.Space, out var wasPressed);

            if (pressed && !wasPressed)
                eventBus.Publish(new JumpAction(InputEventType.Pressed));

            _wasPressed[KeyCode.Space] = pressed;
        }

        private void PublishBlock(IInputEventsBus eventBus)
        {
            bool pressed = UnityEngine.Input.GetKey(KeyCode.Q);
            _wasPressed.TryGetValue(KeyCode.Q, out var wasPressed);

            if (pressed != wasPressed)
            {
                eventBus.Publish(new BlockAction(pressed ?
                    InputEventType.Pressed : InputEventType.Released));

                _wasPressed[KeyCode.Q] = pressed;
            }
        }
    }
}

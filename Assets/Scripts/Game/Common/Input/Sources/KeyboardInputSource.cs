using System.Collections.Generic;
using UnityEngine;

namespace Game.Common.Input
{
    public class KeyboardSource : IInputSource
    {
        public int  Priority { get; }
        public bool IsActive { get; set; } = true;

        private readonly List<IActionInput> _actionBuffer = new(4);
        private bool _wasSpacePressed;

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

        public IReadOnlyList<IActionInput> GetActionInputs()
        {
            _actionBuffer.Clear();

            bool spacePressed = UnityEngine.Input.GetKey(KeyCode.Space);

            if (spacePressed && !_wasSpacePressed)
                _actionBuffer.Add(new JumpAction(InputEventType.Pressed));
            else if (!spacePressed && _wasSpacePressed)
                _actionBuffer.Add(new JumpAction(InputEventType.Released));
            else if (spacePressed)
                _actionBuffer.Add(new JumpAction(InputEventType.Held));

            _wasSpacePressed = spacePressed;

            return _actionBuffer;
        }
    }
}
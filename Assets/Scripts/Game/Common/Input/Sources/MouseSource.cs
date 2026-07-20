using System.Collections.Generic;
using UnityEngine;

namespace Game.Common.Input
{
    public sealed class MouseSource : IInputSource
    {
        public int  Priority { get; }
        public bool IsActive { get; set; } = true;

        private float _sensitivity = 2f;
        private readonly List<IActionInput> _actionBuffer = new(4);
        private bool _wasLmbPressed;
        private bool _wasRmbPressed;

        public MouseSource(int priority)
            => Priority = priority;

        public MoveInput GetMoveInput()
            => MoveInput.None;

        public LookInput GetLookInput()
        {
            float mouseX = UnityEngine.Input.GetAxisRaw("Mouse X");
            float mouseY = UnityEngine.Input.GetAxisRaw("Mouse Y");

            if (Mathf.Approximately(mouseX, 0f) && Mathf.Approximately(mouseY, 0f))
                return LookInput.None;

            Vector2 delta = new(mouseX * _sensitivity, mouseY * _sensitivity);
            return new LookInput(delta);
        }

        public IReadOnlyList<IActionInput> GetActionInputs()
        {
            _actionBuffer.Clear();

            bool lmbPressed = UnityEngine.Input.GetMouseButton(0);
            if (lmbPressed && !_wasLmbPressed)
                _actionBuffer.Add(new SimpleAttackAction(InputEventType.Pressed));
            else if (!lmbPressed && _wasLmbPressed)
                _actionBuffer.Add(new SimpleAttackAction(InputEventType.Released));
            else if (lmbPressed)
                _actionBuffer.Add(new SimpleAttackAction(InputEventType.Held));
            _wasLmbPressed = lmbPressed;

            bool rmbPressed = UnityEngine.Input.GetMouseButton(1);
            if (rmbPressed && !_wasRmbPressed)
                _actionBuffer.Add(new ChargedAttackAction(InputEventType.Pressed));
            else if (!rmbPressed && _wasRmbPressed)
                _actionBuffer.Add(new ChargedAttackAction(InputEventType.Released));
            else if (rmbPressed)
                _actionBuffer.Add(new ChargedAttackAction(InputEventType.Held));
            _wasRmbPressed = rmbPressed;

            return _actionBuffer;
        }
    }
}
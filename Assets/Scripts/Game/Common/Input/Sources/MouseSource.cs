using UnityEngine;

namespace Game.Common.Input
{
    public sealed class MouseSource : IInputSource
    {
        public int  Priority { get; }
        public bool IsActive { get; set; } = true;

        private float _sensitivity = 2f;
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

        public void PublishActions(IInputEventsBus eventBus)
        {
            bool lmbPressed = UnityEngine.Input.GetMouseButton(0);
            if (lmbPressed && !_wasLmbPressed)
                eventBus.Publish(new SimpleAttackAction(InputEventType.Pressed));
            else if (!lmbPressed && _wasLmbPressed)
                eventBus.Publish(new SimpleAttackAction(InputEventType.Released));
            _wasLmbPressed = lmbPressed;

            bool rmbPressed = UnityEngine.Input.GetMouseButton(1);
            if (rmbPressed && !_wasRmbPressed)
                eventBus.Publish(new PowerAttackAction(InputEventType.Pressed));
            else if (!rmbPressed && _wasRmbPressed)
                eventBus.Publish(new PowerAttackAction(InputEventType.Released));
            _wasRmbPressed = rmbPressed;
        }
    }
}

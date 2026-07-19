using UnityEngine;

namespace Game.Character
{
    public class MovementController : IMovementController
    {
        private ICharacterView _view;
        private Vector3        _moveDirection;

        private bool _jumpRequested;
        private bool _isGrounded;

        private float _verticalVelocity;

        public void Initialize(ICharacterView view)
        {
            _view       = view;
            _isGrounded = CheckGrounded();
        }

        public void SetMoveDirection(Vector3 direction)
            => _moveDirection = direction;

        public void Jump()
        {
            if (_isGrounded)
                _jumpRequested = true;
        }

        public void FixedTick()
        {
            if (_view == null)
                return;

            _isGrounded = CheckGrounded();
            
            ApplyGravity();
            ApplyMovement();
            ApplyJump();

            _view.Rigidbody.velocity = new Vector3(
                _view.Rigidbody.velocity.x,
                _verticalVelocity,
                _view.Rigidbody.velocity.z
            );
        }

        private void ApplyMovement()
        {
            Vector3 targetVelocity = _moveDirection * _view.Data.Movement.RunSpeed;
            Vector3 velocityChange = targetVelocity - _view.Rigidbody.velocity;
            velocityChange.y = 0;

            float acceleration = _isGrounded
                ? _view.Data.Movement.Acceleration
                : _view.Data.Movement.Acceleration * _view.Data.Movement.AirControlFactor;

            if (_moveDirection.magnitude < 0.01f)
            {
                velocityChange   = -_view.Rigidbody.velocity;
                velocityChange.y = 0;
                acceleration     = _view.Data.Movement.Deceleration;
            }

            _view.Rigidbody.AddForce(velocityChange * acceleration, ForceMode.Force);

            if (_view.Rigidbody.velocity.magnitude > _view.Data.Movement.RunSpeed && _isGrounded)
            {
                Vector3 clampedVelocity = _view.Rigidbody.velocity;
                clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -_view.Data.Movement.RunSpeed, _view.Data.Movement.RunSpeed);
                clampedVelocity.z = Mathf.Clamp(clampedVelocity.z, -_view.Data.Movement.RunSpeed, _view.Data.Movement.RunSpeed);
                _view.Rigidbody.velocity = clampedVelocity;
            }
        }

        private void ApplyGravity()
        {
            if (_isGrounded && _verticalVelocity < 0)
                _verticalVelocity = -2f;
            else
            {
                float gravity = Physics.gravity.y * _view.Data.Movement.AirborneGravityMultiplier;
                _verticalVelocity += gravity * Time.fixedDeltaTime;
            }
        }

        private void ApplyJump()
        {
            if (_jumpRequested && _isGrounded)
            {
                _verticalVelocity = _view.Data.Movement.JumpImpulse;
                _jumpRequested    = false;
                _isGrounded       = false;
            }
        }

        private bool CheckGrounded()
        {
            Vector3 origin = _view.Transform.position + Vector3.up * 0.1f;
            float distance = 0.25f;
            float radius   = 0.3f;

            return Physics.SphereCast(origin, radius, Vector3.down, out _, distance);
        }
    }
}
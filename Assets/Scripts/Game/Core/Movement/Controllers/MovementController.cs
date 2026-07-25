using Game.Core.Character;
using UnityEngine;

namespace Game.Core.Movement
{
    public class MovementController : IMovementController
    {
        private readonly Rigidbody        _rigidbody;
        private readonly Transform        _transform;
        private readonly MovementSettings _settings;
        
        private Vector3 _moveDirection;
        private Vector3 _airVelocity;

        private bool _jumpRequested;
        private bool _isGrounded;
    
        private float   _verticalVelocity;
        private Vector3 _horizontalVelocity;

        public bool IsGrounded => _isGrounded;

        public MovementController(
            Transform        transform,
            Rigidbody        rigidbody,
            MovementSettings settings
        )
        {
            _transform = transform;
            _rigidbody = rigidbody;
            _settings  = settings;
        }
        
        public void SetMoveDirection(Vector3 direction)
            => _moveDirection = direction.normalized;
        
        public void Jump()
        {
            if (_isGrounded)
                _jumpRequested = true;
        }
        
        public void FixedTick()
        {
            if (_rigidbody == null)
                return;

            _isGrounded = CheckGrounded();
            
            ApplyGravity();
            UpdateHorizontalMovement();
            
            if (_jumpRequested)
                ApplyJump();
            
            ApplyFinalVelocity();
        }

        private void UpdateHorizontalMovement()
        {
            if (_isGrounded)
                UpdateGroundMovement();
            else
                UpdateAirMovement();
        }

        private void UpdateGroundMovement()
        {
            Vector3 worldDirection = _transform.TransformDirection(_moveDirection);
            
            float currentSpeed = _horizontalVelocity.magnitude;
            float targetSpeed  = worldDirection.magnitude > 0.01f ? _settings.RunSpeed : 0f;

            if (targetSpeed > 0.01f)
            {
                float newSpeed = Mathf.MoveTowards(
                    currentSpeed, targetSpeed, _settings.Acceleration * Time.fixedDeltaTime);
                
                _horizontalVelocity = worldDirection * newSpeed;
            }
            else
            {
                float newSpeed = Mathf.MoveTowards(
                    currentSpeed, 0f, _settings.Deceleration * Time.fixedDeltaTime);

                _horizontalVelocity = currentSpeed > 0.01f ?
                    _horizontalVelocity.normalized * newSpeed : Vector3.zero;
            }
            
            _airVelocity = _horizontalVelocity;
        }

        private void UpdateAirMovement()
        {
            Vector3 worldDirection = _transform.TransformDirection(_moveDirection);
            
            if (worldDirection.magnitude > 0.01f)
            {
                float currentSpeed = _airVelocity.magnitude;
                
                if (currentSpeed < 1f)
                {
                    float targetSpeed  = _settings.RunSpeed * _settings.AirControlFactor;
                    float acceleration = _settings.Acceleration * _settings.AirControlFactor;
                    
                    float newSpeed = Mathf.MoveTowards(
                        currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
                    
                    _airVelocity = worldDirection * newSpeed;
                }
                else
                {
                    Vector3 targetVelocity = worldDirection * currentSpeed;
                    float acceleration = _settings.Acceleration * _settings.AirControlFactor;

                    Vector3 diff = targetVelocity - _airVelocity;
                    float diffMagnitude = diff.magnitude;
                    
                    if (diffMagnitude > 0.01f)
                    {
                        float change = Mathf.Min(acceleration * Time.fixedDeltaTime, diffMagnitude);
                        _airVelocity += diff.normalized * change;
                    }
                }
            }
            
            _horizontalVelocity = _airVelocity;
        }
        
        private void ApplyGravity()
        {
            if (_isGrounded && _verticalVelocity <= 0)
            {
                _verticalVelocity = -2f;
            }
            else
            {
                float gravity = Physics.gravity.y * _settings.AirborneGravityMultiplier;
                _verticalVelocity += gravity * Time.fixedDeltaTime;
            }
        }
        
        private void ApplyJump()
        {
            if (!_jumpRequested || !_isGrounded)
                return;
            
            _verticalVelocity = _settings.JumpImpulse;
            _airVelocity      = _horizontalVelocity;
            
            _jumpRequested = false;
            _isGrounded    = false;
        }
        
        private void ApplyFinalVelocity()
        {
            _rigidbody.velocity = new Vector3(
                _horizontalVelocity.x, _verticalVelocity, _horizontalVelocity.z);
        }
        
        private bool CheckGrounded()
            => Physics.Raycast(_rigidbody.position, Vector3.down, 0.2f);
        
        public void ApplyExternalForce(Vector3 force)
        {
            _horizontalVelocity += new Vector3(force.x, 0, force.z);
            _verticalVelocity += force.y;
            _isGrounded = false;
        }
    }
}
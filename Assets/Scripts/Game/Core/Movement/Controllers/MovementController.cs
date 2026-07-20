using Game.Core.Character;
using UnityEngine;
using Zenject;

namespace Game.Core.Movement
{
    public class MovementController : IMovementController, IFixedTickable
    {
        private Rigidbody        _rigidbody;
        private MovementSettings _settings;
        
        private Vector3 _moveDirection;
        private bool    _jumpRequested;
        private bool    _isGrounded;
    
        private float   _verticalVelocity;
        private Vector3 _horizontalVelocity;

        public bool IsGrounded => _isGrounded;
        
        public void Initialize(ICharacterView view)
        {
            _rigidbody = view.Rigidbody;
            _settings  = view.Data.Movement;
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
            float targetSpeed = _moveDirection.magnitude > 0.01f ? 
                _settings.RunSpeed : 0f;
            
            float currentSpeed = _horizontalVelocity.magnitude;
            
            if (targetSpeed > 0.01f)
            {
                float acceleration = _isGrounded ? 
                    _settings.Acceleration : 
                    _settings.Acceleration * _settings.AirControlFactor;
                    
                float newSpeed = Mathf.MoveTowards(
                    currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
                
                _horizontalVelocity = _moveDirection * newSpeed;
            }
            else
            {
                float deceleration = _isGrounded ? 
                    _settings.Deceleration : 
                    _settings.Deceleration * _settings.AirControlFactor;
                    
                float newSpeed = Mathf.MoveTowards(
                    currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
                
                if (currentSpeed > 0.01f)
                    _horizontalVelocity = _horizontalVelocity.normalized * newSpeed;
                else
                    _horizontalVelocity = Vector3.zero;
            }
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
            _jumpRequested    = false;
            _isGrounded       = false;
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
            _rigidbody.AddForce(force, ForceMode.Impulse);
            
            _isGrounded       = false;
            _verticalVelocity = force.y;
        }
    }
}
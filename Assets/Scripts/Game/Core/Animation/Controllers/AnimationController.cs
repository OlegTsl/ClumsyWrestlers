using Game.Core.Character;
using UnityEngine;

namespace Game.Core.Animation
{
    public class AnimationController : IAnimationController
    {
        private Animator  _animator;
        private Transform _transform;
        
        private static readonly int MoveX           = Animator.StringToHash("MoveX");
        private static readonly int MoveY           = Animator.StringToHash("MoveY");
        private static readonly int Speed           = Animator.StringToHash("Speed");
        private static readonly int IsGroundedParam = Animator.StringToHash("IsGrounded");
        private static readonly int JumpTrigger     = Animator.StringToHash("Jump");
        
        public void Initialize(ICharacterView view)
        {
            _animator  = view.Animator;
            _transform = view.Transform;
        }
        
        public void UpdateMovementState(Vector3 velocity, bool isGrounded)
        {
            if (_animator == null)
                return;
            
            Vector3 localVelocity = _transform.InverseTransformDirection(velocity);
            localVelocity.y = 0;
            
            float speed = localVelocity.magnitude;
            if (speed < 0.1f)
            {
                speed         = 0f;
                localVelocity = Vector3.zero;
            }

            float moveX = 0f;
            float moveY = 0f;
            
            if (speed > 0f)
            {
                Vector3 normalizedVelocity = localVelocity / speed;
                
                float absZ = Mathf.Abs(normalizedVelocity.z);
                float absX = Mathf.Abs(normalizedVelocity.x);
                
                if (absZ > absX)
                {
                    moveY = normalizedVelocity.z > 0 ? 1f : -1f;
                    moveX = 0f;
                }
                else
                {
                    moveX = normalizedVelocity.x > 0 ? 1f : -1f;
                    moveY = 0f;
                }
            }
            
            _animator.SetFloat(MoveX, moveX, 0.03f, Time.deltaTime);
            _animator.SetFloat(MoveY, moveY, 0.03f, Time.deltaTime);
            _animator.SetFloat(Speed, speed, 0.03f, Time.deltaTime);
            _animator.SetBool(IsGroundedParam, isGrounded);
        }

        public void TriggerJump()
        {
            if (_animator != null)
                _animator.SetTrigger(JumpTrigger);
        }
    }
}
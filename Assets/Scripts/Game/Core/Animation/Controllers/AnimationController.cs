using System;
using Game.Core.Events;
using UnityEngine;

namespace Game.Core.Animation
{
    public class AnimationController : IAnimationController, IDisposable
    {
        private readonly Transform     _transform;
        private readonly Animator      _animator;
        private readonly GameEventsBus _eventsBus;

        public AnimationController(
            Transform     transform,
            Animator      animator,
            GameEventsBus eventsBus
        )
        {
            _transform = transform;
            _animator  = animator;
            _eventsBus = eventsBus;

            _eventsBus.Subscribe<OnPunchStartedEvent>(OnPunchStarted);
        }
        
        public void UpdateMovementState(Vector3 velocity, bool isMoving, bool isGrounded)
        {
            if (_animator == null)
                return;
            
            Vector3 worldVelocity = velocity;
            worldVelocity.y = 0;
            
            float speed = worldVelocity.magnitude;
            
            if (speed < 0.1f)
            {
                speed = 0f;
                worldVelocity = Vector3.zero;
            }
            
            float moveX = 0f;
            float moveY = 0f;
            
            if (speed > 0f)
            {
                Vector3 localVelocity = _transform.InverseTransformDirection(worldVelocity.normalized);
                float absX = Mathf.Abs(localVelocity.x);  

                if (absX > 0.3f)
                {
                    moveX = localVelocity.x > 0 ? 1f : -1f;
                    moveY = 0f;
                }
                else
                {
                    moveY = localVelocity.z > 0 ? 1f : -1f;
                    moveX = 0f;
                }
            }
            
            _animator.SetFloat(AnimationData.MoveX, moveX, 0.05f, Time.deltaTime);
            _animator.SetFloat(AnimationData.MoveY, moveY, 0.05f, Time.deltaTime);
            _animator.SetFloat(AnimationData.Speed, speed, 0.05f, Time.deltaTime);

            _animator.SetBool(AnimationData.Moving,   isMoving);
            _animator.SetBool(AnimationData.Grounded, isGrounded);
        }

        private void OnPunchStarted(OnPunchStartedEvent evt)
            => TriggerPunch();

        public void TriggerJump()
        {
            if (_animator != null)
                _animator.SetTrigger(AnimationData.JumpTrigger);
        }

        public void TriggerPunch()
        {
            if (_animator != null)
                _animator.SetTrigger(AnimationData.PunchTrigger);
        }

        public void Dispose()
            => _eventsBus.Unsubscribe<OnPunchStartedEvent>(OnPunchStarted);
    }
}
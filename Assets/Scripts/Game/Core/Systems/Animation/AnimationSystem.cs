using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.Data;
using Game.Core.GameEvents;
using UnityEngine;
using Zenject;

namespace Game.Core.Systems
{
    public class AnimationSystem : IAnimationSystem, ITickable
    {
        private readonly GameEventsBus     _events;
        private readonly ICharacterContext _context;
        private readonly Dictionary<Guid, AnimationState> _states = new();

        public AnimationSystem(
            GameEventsBus     events,
            ICharacterContext context
        )
        {
            _events  = events;
            _context = context;
            
            _events.Subscribe<OnJumpEvent>(OnJump);
            _events.Subscribe<OnFallEvent>(OnFall);
            _events.Subscribe<OnMoveEvent>(OnMove);
            _events.Subscribe<OnSimpleAttackStartedEvent>(OnSimpleAttackStarted);
            _events.Subscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);

            _context.OnCharacterAdded   += Register;
            _context.OnCharacterRemoved += Unregister;
        }

        private void Register(Guid id)
            => _states[id] = new AnimationState();

        private void Unregister(Guid id)
            => _states.Remove(id);

        private void OnJump(OnJumpEvent evt)
        {
            var character = _context.GetCharacter(evt.CharacterID);
            if (character == null || !character.Enabled)
                return;

            character.SetAnimatorTrigger(AnimationData.JumpTrigger);
        }

        private void OnFall(OnFallEvent evt)
        {
            var character = _context.GetCharacter(evt.CharacterID);
            if (character == null || !character.Enabled)
                return;

            character.SetAnimatorTrigger(AnimationData.FallTrigger);
        }

        private void OnMove(OnMoveEvent evt)
        {
            var character = _context.GetCharacter(evt.CharacterID);
            if (character == null || !character.Enabled)
                return;

            character.SetAnimatorBool(AnimationData.MoveInput,
                evt.Direction != Vector3.zero);
        }

        private void OnSimpleAttackStarted(OnSimpleAttackStartedEvent evt)
        {
            var character = _context.GetCharacter(evt.CharacterID);
            if (character == null || !character.Enabled)
                return;

            character.SetAnimatorTrigger(AnimationData.PunchTrigger);
        }

        private void OnPowerAttackStarted(OnPowerAttackStartedEvent evt)
        {
            var character = _context.GetCharacter(evt.CharacterID);
            if (character == null || !character.Enabled)
                return;

            character.SetAnimatorTrigger(AnimationData.PowerPunchTrigger);
        }

        public void Tick()
        {
            var characters = _context.AllCharacters;
            for (int idx = 0; idx < characters.Count; idx++)
            {
                var character = characters[idx];
                if (!character.Enabled)
                    continue;

                UpdateMovementAnimation(character);
            }
        }

        private void UpdateMovementAnimation(ICharacter character)
        {
            var velocity = character.GetVelocity();

            Vector3 worldVelocity = velocity;
            worldVelocity.y       = 0;
            
            float speed = worldVelocity.magnitude;
            if (speed < 0.1f)
            {
                speed         = 0f;
                worldVelocity = Vector3.zero;
            }
            
            float moveX = 0f;
            float moveY = 0f;
            
            if (speed > 0f)
            {
                Vector3 localVelocity = character.InverseTransformDirection(worldVelocity.normalized);
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

            character.SetAnimatorFloat(AnimationData.MoveX, moveX, 0.05f, Time.deltaTime);
            character.SetAnimatorFloat(AnimationData.MoveY, moveY, 0.05f, Time.deltaTime);
            character.SetAnimatorFloat(AnimationData.Speed, speed, 0.05f, Time.deltaTime);
            character.SetAnimatorBool(AnimationData.Grounded, character.IsGrounded());
        }

        public void Dispose()
        {
            _events.Unsubscribe<OnJumpEvent>(OnJump);
            _events.Unsubscribe<OnFallEvent>(OnFall);
            _events.Unsubscribe<OnMoveEvent>(OnMove);
            _events.Unsubscribe<OnSimpleAttackStartedEvent>(OnSimpleAttackStarted);
            _events.Unsubscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);
        }
    }
}
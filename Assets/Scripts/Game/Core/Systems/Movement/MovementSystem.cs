using System;
using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.GameEvents;
using UnityEngine;
using Zenject;

namespace Game.Core.Systems
{
    public class MovementSystem : IMovementSystem, IFixedTickable
    {
        private readonly GameEventsBus     _gameEventsBus;
        private readonly ICharacterContext _context;
        private readonly Dictionary<Guid, MovementState> _states = new();

        public MovementSystem(
            GameEventsBus     gameEventsBus,
            ICharacterContext context
        )
        {
            _gameEventsBus = gameEventsBus;
            _context       = context;
            
            _gameEventsBus.Subscribe<OnMoveEvent>(OnMove);
            _gameEventsBus.Subscribe<OnJumpEvent>(OnJump);

            _context.OnCharacterAdded   += Register;
            _context.OnCharacterRemoved += Unregister;
        }

        private void Register(Guid id)
            => _states[id] = new MovementState();

        private void Unregister(Guid id)
            => _states.Remove(id);

        private void OnMove(OnMoveEvent evt)
        {
            var character = _context.GetCharacter(evt.CharacterID);
            if (character == null || !character.Enabled)
                return;

            if (_states.TryGetValue(evt.CharacterID, out var state))
                state.MoveDirection = evt.Direction;
        }

        private void OnJump(OnJumpEvent evt)
        {
            var character = _context.GetCharacter(evt.CharacterID);
            if (character == null || !character.Enabled)
                return;

            if (_states.TryGetValue(evt.CharacterID, out var state))
                state.JumpRequested = character.IsGrounded();
        }

        public void FixedTick()
        {
            var characters = _context.AllCharacters;

            for (int idx = 0; idx < characters.Count; idx++)
            {
                var character = characters[idx];
                if (!character.Enabled)
                    continue;

                if (!_states.TryGetValue(character.CharacterID, out var state))
                    continue;

                var settings = character.Data.Movement;

                TrackAirTime(character, state, settings);
                ApplyGravity(character, state, settings);

                UpdateHorizontalMovement(character, state, settings);

                if (state.JumpRequested)
                    ApplyJump(state, settings);

                ApplyFinalVelocity(state, character);
            }
        }

        private void TrackAirTime(
            ICharacter       character,
            MovementState    state,
            MovementSettings settings
        )
        {
            if (character.IsGrounded())
            {
                state.AirTime   = 0f;
                state.IsFalling = false;
                return;
            }

            state.AirTime += Time.fixedDeltaTime;

            if (!state.IsFalling && state.AirTime >= settings.FallingDelay)
            {
                state.IsFalling = true;
                _gameEventsBus.Publish(new OnFallEvent(character.CharacterID));
            }
        }

        private void ApplyGravity(
            ICharacter       character,
            MovementState    state,
            MovementSettings settings
        )
        {
            bool isGrounded = character.IsGrounded();

            if (isGrounded && state.VerticalVelocity <= 0)
            {
                state.VerticalVelocity = -2f;
            }
            else
            {
                float gravity = Physics.gravity.y * settings.AirborneGravityMultiplier;
                state.VerticalVelocity += gravity * Time.fixedDeltaTime;
            }
        }

        private void UpdateHorizontalMovement(
            ICharacter       character,
            MovementState    state,
            MovementSettings settings
        )
        {
            if (character.IsGrounded())
                UpdateGroundMovement(character, state, settings);
            else
                UpdateAirMovement(character, state, settings);
        }

        private void UpdateGroundMovement(
            ICharacter       character,
            MovementState    state,
            MovementSettings settings
        )
        {
            Vector3 worldDirection = character.TransformDirection(state.MoveDirection);
            
            float currentSpeed = state.HorizontalVelocity.magnitude;
            float targetSpeed  = worldDirection.magnitude > 0.01f ? settings.RunSpeed : 0f;

            if (targetSpeed > 0.01f)
            {
                float newSpeed = Mathf.MoveTowards(
                    currentSpeed, targetSpeed, settings.Acceleration * Time.fixedDeltaTime);
                    
                state.HorizontalVelocity = worldDirection * newSpeed;
            }
            else
            {
                float newSpeed = Mathf.MoveTowards(
                    currentSpeed, 0f, settings.Deceleration * Time.fixedDeltaTime);
                
                state.HorizontalVelocity = currentSpeed > 0.01f ?
                    state.HorizontalVelocity.normalized * newSpeed : Vector3.zero;
            }

            state.AirVelocity = state.HorizontalVelocity;
        }

        private void UpdateAirMovement(
            ICharacter       character,
            MovementState    state,
            MovementSettings settings
        )
        {
            Vector3 worldDirection = character.TransformDirection(state.MoveDirection);

            if (worldDirection.magnitude > 0.01f)
            {
                float currentSpeed = state.AirVelocity.magnitude;

                if (currentSpeed < 1f)
                {
                    float targetSpeed  = settings.RunSpeed * settings.AirControlFactor;
                    float acceleration = settings.Acceleration * settings.AirControlFactor;
                    float newSpeed     = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
                    
                    state.AirVelocity = worldDirection * newSpeed;
                }
                else
                {
                    Vector3 targetVelocity = worldDirection * currentSpeed;
                    Vector3 diff           = targetVelocity - state.AirVelocity;

                    float acceleration  = settings.Acceleration * settings.AirControlFactor;
                    float diffMagnitude = diff.magnitude;

                    if (diffMagnitude > 0.01f)
                    {
                        float change = Mathf.Min(acceleration * Time.fixedDeltaTime, diffMagnitude);
                        state.AirVelocity += diff.normalized * change;
                    }
                }
            }

            state.HorizontalVelocity = state.AirVelocity;
        }

        private void ApplyJump(MovementState state, MovementSettings settings)
        {
            if (!state.JumpRequested)
                return;

            state.VerticalVelocity = settings.JumpImpulse;
            state.AirVelocity      = state.HorizontalVelocity;
            state.JumpRequested    = false;
        }

        private void ApplyFinalVelocity(MovementState state, ICharacter character)
        {
            character.ApplyVelocity(new Vector3(
                state.HorizontalVelocity.x,
                state.VerticalVelocity,
                state.HorizontalVelocity.z));
        }

        private void ApplyExternalForce(Guid id, Vector3 force)
        {
            if (_states.TryGetValue(id, out var state))
            {
                state.HorizontalVelocity += new Vector3(force.x, 0, force.z);
                state.VerticalVelocity   += force.y;
            }
        }

        public void Dispose()
        {
            _gameEventsBus.Unsubscribe<OnMoveEvent>(OnMove);
            _gameEventsBus.Unsubscribe<OnJumpEvent>(OnJump);

            _context.OnCharacterAdded   -= Register;
            _context.OnCharacterRemoved -= Unregister;
        }
    }
}
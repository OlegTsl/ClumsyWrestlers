using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.Entities;
using Game.Core.GameEvents;
using UnityEngine;
using Zenject;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Systems
{
    public class MovementSystem : IMovementSystem, IFixedTickable
    {
        private readonly IGameEventsBus     _gameEventsBus;
        private readonly ICharacterContext _context;
        private readonly Dictionary<EntityId, MovementState> _states = new(16);

        public MovementSystem(
            IGameEventsBus     gameEventsBus,
            ICharacterContext context
        )
        {
            _gameEventsBus = gameEventsBus;
            _context       = context;
            
            _gameEventsBus.Subscribe<OnMoveEvent>(OnMove);
            _gameEventsBus.Subscribe<OnJumpEvent>(OnJump);
            _gameEventsBus.Subscribe<OnForceEvent>(OnForce);

            _context.OnCharacterAdded   += Register;
            _context.OnCharacterRemoved += Unregister;

            RegisterExistingCharacters();
        }

        private void RegisterExistingCharacters()
        {
            var characters = _context.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                Register(characters[i].CharacterID);
            }
        }

        private void Register(EntityId id)
        {
            if (!_states.ContainsKey(id))
                _states[id] = new MovementState();
        }

        private void Unregister(EntityId id)
            => _states.Remove(id);

        private void OnMove(OnMoveEvent evt)
        {
            var model = _context.GetModel(evt.CharacterID);
            if (model == null)
                return;

            ICharacterMovementRuntimeState movement =
                model.GetState<ICharacterMovementRuntimeState>();
            if (!movement.Enabled)
                return;

            if (_states.TryGetValue(evt.CharacterID, out var state) &&
                movement.IsMovable)
                state.MoveDirection = evt.Direction;
        }

        private void OnJump(OnJumpEvent evt)
        {
            var model = _context.GetModel(evt.CharacterID);
            if (model == null)
                return;

            ICharacterMovementRuntimeState movement =
                model.GetState<ICharacterMovementRuntimeState>();
            if (!movement.Enabled)
                return;

            if (_states.TryGetValue(evt.CharacterID, out var state) &&
                movement.IsMovable)
                state.JumpRequested = movement.IsGrounded;
        }

        public void FixedTick()
        {
            var characters = _context.AllCharacters;

            for (int idx = 0; idx < characters.Count; idx++)
            {
                ICharacterMovementRuntimeState character =
                    characters[idx].GetState<ICharacterMovementRuntimeState>();
                if (!character.Enabled)
                    continue;

                if (!_states.TryGetValue(character.CharacterID, out var state))
                    continue;

                HandleMovementLock(character, state);

                var settings = character.Data.Movement;

                UpdateHorizontalMovement(character, state, settings);
                UpdateRotation(character, state);

                if (state.JumpRequested)
                    ApplyJump(state, settings);

                TrackAirTime(character, state, settings);
                ApplyGravity(character, state, settings);

                ApplyFinalVelocity(state, character);
            }
        }

        private void HandleMovementLock(
            ICharacterMovementRuntimeState model,
            MovementState state)
        {
            if (model.IsMovable)
                return;

            state.MoveDirection = Vector3.zero;
            state.JumpRequested = false;

            if (model.IsGrounded && state.VerticalVelocity <= 0f)
            {
                state.HorizontalVelocity = Vector3.zero;
                state.AirVelocity = Vector3.zero;
            }
        }

        private void TrackAirTime(
            ICharacterMovementRuntimeState model,
            MovementState    state,
            MovementSettings settings
        )
        {
            if (model.IsGrounded)
            {
                state.AirTime   = 0f;
                state.IsFalling = false;
                return;
            }

            state.AirTime += Time.fixedDeltaTime;

            if (!state.IsFalling && state.AirTime >= settings.FallingDelay)
            {
                state.IsFalling = true;
                _gameEventsBus.Publish(new OnFallEvent(model.CharacterID));
            }
        }

        private void ApplyGravity(
            ICharacterMovementRuntimeState model,
            MovementState    state,
            MovementSettings settings
        )
        {
            bool isGrounded = model.IsGrounded;
            if (isGrounded && state.VerticalVelocity <= 0)
            {
                state.VerticalVelocity = -2f;
            }
            else
            {
                float multiplier = model.IsMovable ? settings.GravityMultiplier : 1f;
                float gravity    = Physics.gravity.y * multiplier;
                state.VerticalVelocity += gravity * Time.fixedDeltaTime;
            }
        }

        private void UpdateHorizontalMovement(
            ICharacterMovementRuntimeState model,
            MovementState    state,
            MovementSettings settings
        )
        {
            if (!model.IsMovable)
                return;

            if (model.IsGrounded)
                UpdateGroundMovement(state, settings);
            else
                UpdateAirMovement(state, settings);
        }

        private void UpdateGroundMovement(MovementState state, MovementSettings settings)
        {
            Vector3 worldDirection = state.MoveDirection;
            
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

        private void UpdateAirMovement(MovementState state, MovementSettings settings)
        {
            Vector3 worldDirection = state.MoveDirection;

            if (worldDirection.magnitude > 0.01f)
            {
                float currentSpeed = state.AirVelocity.magnitude;

                if (currentSpeed < 1f)
                {
                    float targetSpeed  = settings.RunSpeed     * settings.AirControlFactor;
                    float acceleration = settings.Acceleration * settings.AirControlFactor;
                    float newSpeed = Mathf.MoveTowards(
                        currentSpeed,
                        targetSpeed,
                        acceleration * Time.fixedDeltaTime);
                    
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

        private void UpdateRotation(
            ICharacterMovementRuntimeState model,
            MovementState state)
        {
            if (state.MoveDirection.magnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(state.MoveDirection);
                model.SetRotation(Quaternion.RotateTowards(
                    model.Rotation,
                    targetRotation,
                    model.Data.Movement.RotationSpeed * Time.fixedDeltaTime
                ));
            }
        }

        private void ApplyJump(MovementState state, MovementSettings settings)
        {
            if (!state.JumpRequested)
                return;

            state.VerticalVelocity = settings.JumpImpulse;
            state.AirVelocity      = state.HorizontalVelocity;
            state.JumpRequested    = false;
        }

        private void ApplyFinalVelocity(
            MovementState state,
            ICharacterMovementRuntimeState model)
        {
            model.SetVelocity(new Vector3(
                state.HorizontalVelocity.x,
                state.VerticalVelocity,
                state.HorizontalVelocity.z));
        }

        private void OnForce(OnForceEvent evt)
        {
            ICharacterModel model = _context.GetModel(evt.CharacterID);
            if (model == null ||
                !_states.TryGetValue(evt.CharacterID, out var state))
            {
                return;
            }

            ICharacterMovementRuntimeState movement =
                model.GetState<ICharacterMovementRuntimeState>();
            if (!movement.Enabled)
            {
                return;
            }

            state.HorizontalVelocity = new Vector3(evt.Force.x, 0f, evt.Force.z);
            state.VerticalVelocity = evt.Force.y;
            state.AirVelocity = state.HorizontalVelocity;

            movement.SetVelocity(evt.Force);
        }

        public void Dispose()
        {
            _gameEventsBus.Unsubscribe<OnMoveEvent>(OnMove);
            _gameEventsBus.Unsubscribe<OnJumpEvent>(OnJump);
            _gameEventsBus.Unsubscribe<OnForceEvent>(OnForce);

            _context.OnCharacterAdded   -= Register;
            _context.OnCharacterRemoved -= Unregister;
        }
    }
}

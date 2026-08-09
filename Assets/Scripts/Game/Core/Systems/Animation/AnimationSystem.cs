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
            _events.Subscribe<OnAttackStartedEvent>(OnAttackStarted);

            _context.OnCharacterAdded   += Register;
            _context.OnCharacterRemoved += Unregister;
        }

        private void Register(Guid id)
            => _states[id] = new AnimationState();

        private void Unregister(Guid id)
            => _states.Remove(id);

        private void OnJump(OnJumpEvent evt)
        {
            var character = _context.GetModel(evt.CharacterID);
            if (character == null || !character.Enabled)
                return;

            character.SetAnimatorTrigger(AnimationData.JumpTrigger);
        }

        private void OnFall(OnFallEvent evt)
        {
            var character = _context.GetModel(evt.CharacterID);
            if (character == null || !character.Enabled)
                return;

            character.SetAnimatorTrigger(AnimationData.FallTrigger);
        }

        private void OnMove(OnMoveEvent evt)
        {
            var character = _context.GetModel(evt.CharacterID);
            if (character == null || !character.Enabled)
                return;

            character.SetAnimatorBool(AnimationData.MoveInput,
                evt.Direction != Vector3.zero);
        }

        private void OnAttackStarted(OnAttackStartedEvent evt)
        {
            switch (evt.Type)
            {
                case AttackType.Simple:
                    OnSimpleAttackStarted(evt.CharacterID);
                    break;
                case AttackType.Power:
                    OnPowerAttackStarted(evt.CharacterID);
                    break;
                default:
                    break;
            }
        }

        private void OnSimpleAttackStarted(Guid characterID)
        {
            var character = _context.GetModel(characterID);
            if (character == null || !character.Enabled)
                return;

            character.SetAnimatorTrigger(AnimationData.PunchTrigger);
        }

        private void OnPowerAttackStarted(Guid characterID)
        {
            var character = _context.GetModel(characterID);
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

        private void UpdateMovementAnimation(ICharacterModel model)
        {
            var velocity = model.GetVelocity();

            Vector3 worldVelocity = velocity;
            worldVelocity.y       = 0;
            
            float speed = worldVelocity.magnitude;
            if (speed < 0.1f)
                speed = 0f;
            
            model.SetAnimatorFloat(AnimationData.Speed, speed, 0.05f, Time.deltaTime);
            model.SetAnimatorBool(AnimationData.Grounded, model.IsGrounded());
        }

        public void Dispose()
        {
            _events.Unsubscribe<OnJumpEvent>(OnJump);
            _events.Unsubscribe<OnFallEvent>(OnFall);
            _events.Unsubscribe<OnMoveEvent>(OnMove);
            _events.Unsubscribe<OnAttackStartedEvent>(OnAttackStarted);
        }
    }
}
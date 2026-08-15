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
            _events.Subscribe<OnHitEvent>(OnHit);
            _events.Subscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);
            _events.Subscribe<OnSimpleAttackStartedEvent>(OnSimpleAttackStarted);
        }

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

        private void OnHit(OnHitEvent evt)
        {
            var character = _context.GetModel(evt.CharacterID);
            if (character == null || !character.Enabled)
                return;

            character.SetAnimatorTrigger(AnimationData.HitTrigger);
        }

        private void OnPowerAttackStarted(OnPowerAttackStartedEvent evt)
        {
            var character = _context.GetModel(evt.CharacterID);
            if (character == null || !character.Enabled)
                return;

            PowerAttackSettings settings = character.Data.Combat.PowerAttack;
            if (settings == null)
                return;

            character.SetAnimatorFloat(AnimationData.PowerAttackSpeed,
                settings.AnimationSpeed, 0f, Time.deltaTime);
            character.SetAnimatorTrigger(AnimationData.PowerAttackTrigger);
        }

        private void OnSimpleAttackStarted(OnSimpleAttackStartedEvent evt)
        {
            var character = _context.GetModel(evt.CharacterID);
            if (character == null || !character.Enabled)
                return;

            SimpleAttackSettings settings = character.Data.Combat.SimpleAttack;
            if (settings == null)
                return;

            character.SetAnimatorFloat(AnimationData.SimpleAttackSpeed,
                settings.AnimationSpeed, 0f, Time.deltaTime);
            character.SetAnimatorBool(AnimationData.MirrorPunch, evt.IsMirrored);
            character.SetAnimatorTrigger(AnimationData.PunchTrigger);
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
            _events.Unsubscribe<OnHitEvent>(OnHit);
            _events.Unsubscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);
            _events.Unsubscribe<OnSimpleAttackStartedEvent>(OnSimpleAttackStarted);
        }
    }
}

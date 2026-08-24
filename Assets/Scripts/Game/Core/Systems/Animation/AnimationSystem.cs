using Game.Core.Character;
using Game.Core.Data;
using Game.Core.Extension;
using Game.Core.GameEvents;
using UnityEngine;
using Zenject;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Systems
{
    public sealed class AnimationSystem : IAnimationSystem, ITickable
    {
        private readonly IGameEventsBus        _events;
        private readonly ICharacterContext     _models;
        private readonly ICharacterViewContext _views;

        public AnimationSystem(
            IGameEventsBus        events,
            ICharacterContext     models,
            ICharacterViewContext views
        )
        {
            _events = events;
            _models = models;
            _views  = views;

            _events.Subscribe<OnJumpEvent>(OnJump);
            _events.Subscribe<OnFallEvent>(OnFall);
            _events.Subscribe<OnMoveEvent>(OnMove);
            _events.Subscribe<OnHitEvent>(OnHit);
            _events.Subscribe<OnBlockEvent>(OnBlock);
            _events.Subscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);
            _events.Subscribe<OnPowerAttackEndedEvent>(OnPowerAttackEnded);
            _events.Subscribe<OnSimpleAttackStartedEvent>(OnSimpleAttackStarted);
            _events.Subscribe<OnAttackHandIkEvent>(OnAttackHandIk);
            _events.Subscribe<OnAttackHandIkClearedEvent>(OnAttackHandIkCleared);
        }

        private void OnJump(OnJumpEvent evt)
            => SetTrigger(evt.CharacterID, AnimationData.JumpTrigger);

        private void OnFall(OnFallEvent evt)
            => SetTrigger(evt.CharacterID, AnimationData.FallTrigger);

        private void OnMove(OnMoveEvent evt)
        {
            ICharacterView view = GetEnabledView(evt.CharacterID);
            view?.SetAnimatorBool(AnimationData.MoveInput, evt.Direction != Vector3.zero);
        }

        private void OnHit(OnHitEvent evt)
            => SetTrigger(evt.CharacterID, AnimationData.HitTrigger);

        private void OnBlock(OnBlockEvent evt)
        {
            ICharacterView view = GetEnabledView(evt.CharacterID);
            view?.SetAnimatorBool(AnimationData.Blocked, evt.Blocked);
        }

        private void OnPowerAttackStarted(OnPowerAttackStartedEvent evt)
        {
            ICharacterModel model = _models.GetModel(evt.CharacterID);
            ICharacterView  view  = GetEnabledView(evt.CharacterID);
            
            if (model == null || view == null)
                return;

            PowerAttackSettings settings = model.Data.Combat.PowerAttack;
            view.SetAnimatorFloat(AnimationData.PowerAttackSpeed,
                settings.AnimationSpeed, 0f, Time.deltaTime);
            view.SetAnimatorBool(AnimationData.IsPowerAttacking, true);
            view.SetAnimatorTrigger(AnimationData.PowerAttackTrigger);
        }

        private void OnPowerAttackEnded(OnPowerAttackEndedEvent evt)
            => _views.GetView(evt.CharacterID)?.SetAnimatorBool(
                AnimationData.IsPowerAttacking, false);

        private void OnSimpleAttackStarted(OnSimpleAttackStartedEvent evt)
        {
            ICharacterModel model = _models.GetModel(evt.CharacterID);
            ICharacterView  view  = GetEnabledView(evt.CharacterID);
            
            if (model == null || view == null)
                return;

            SimpleAttackSettings settings = model.Data.Combat.SimpleAttack;
            view.SetAnimatorFloat(AnimationData.SimpleAttackSpeed,
                settings.AnimationSpeed, 0f, Time.deltaTime);
            view.SetAnimatorBool(AnimationData.MirrorPunch, evt.IsMirrored);
            view.SetAnimatorTrigger(AnimationData.PunchTrigger);
        }

        private void OnAttackHandIk(OnAttackHandIkEvent evt)
        {
            ICharacterView view = GetEnabledView(evt.CharacterID);
            view?.SetAttackHandIk(evt.Hand, evt.Position, evt.Weight);
        }

        private void OnAttackHandIkCleared(OnAttackHandIkClearedEvent evt)
            => _views.GetView(evt.CharacterID)?.ClearAttackHandIk();

        public void Tick()
        {
            var characters = _models.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                ICharacterModel model = characters[i];
                ICharacterPresentationState presentation = model.GetState<ICharacterPresentationState>();
                
                if (!presentation.Enabled)
                    continue;

                ICharacterView view = _views.GetView(model.CharacterID);
                if (view == null)
                    continue;

                Vector3 horizontalVelocity = presentation.Velocity;
                horizontalVelocity.y = 0f;
                
                float speed = horizontalVelocity.sqrMagnitude < 0.01f
                    ? 0f : horizontalVelocity.magnitude;
                
                view.SetAnimatorFloat(AnimationData.Speed, speed, 0.05f, Time.deltaTime);
                view.SetAnimatorBool(AnimationData.Grounded, presentation.IsGrounded);
            }
        }

        private void SetTrigger(EntityId characterId, int trigger)
        {
            ICharacterView view = GetEnabledView(characterId);
            view?.SetAnimatorTrigger(trigger);
        }

        private ICharacterView GetEnabledView(EntityId characterId)
        {
            ICharacterModel model = _models.GetModel(characterId);
            return model != null && model.GetState<ICharacterActivityState>().Enabled
                ? _views.GetView(characterId) : null;
        }

        public void Dispose()
        {
            _events.Unsubscribe<OnJumpEvent>(OnJump);
            _events.Unsubscribe<OnFallEvent>(OnFall);
            _events.Unsubscribe<OnMoveEvent>(OnMove);
            _events.Unsubscribe<OnHitEvent>(OnHit);
            _events.Unsubscribe<OnBlockEvent>(OnBlock);
            _events.Unsubscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);
            _events.Unsubscribe<OnPowerAttackEndedEvent>(OnPowerAttackEnded);
            _events.Unsubscribe<OnSimpleAttackStartedEvent>(OnSimpleAttackStarted);
            _events.Unsubscribe<OnAttackHandIkEvent>(OnAttackHandIk);
            _events.Unsubscribe<OnAttackHandIkClearedEvent>(OnAttackHandIkCleared);
        }
    }
}

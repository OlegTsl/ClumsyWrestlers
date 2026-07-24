using Game.Common.Input;
using Game.Core.Animation;
using Game.Core.Character;
using UnityEngine;

namespace Game.Core.Combat
{
    public class CombatController : ICombatController, System.IDisposable
    {
        private readonly InputEventBus _inputEvents;

        private IAnimationController _animationController;
        private ICharacterView       _view;
        private AttackSettings       _attackSettings;
        private IHitController       _hitController;

        private bool  _isEnabled;
        private bool  _isAttacking;
        private bool  _isHitboxActive;
        private float _attackElapsed;

        public CombatController(InputEventBus inputEvents)
        {
            _inputEvents = inputEvents;
            _inputEvents.Subscribe<SimpleAttackAction>(OnSimpleAttack);
        }

        public void Initialize(
            ICharacterView       view,
            IAnimationController animationController,
            IHitController       hitController
        )
        {
            _view                = view;
            _animationController = animationController;
            _hitController       = hitController;
            _attackSettings      = view.Data.AttackSettings;
            _isEnabled           = true;
        }

        public void FixedTick()
        {
            if (!_isAttacking)
                return;

            _attackElapsed += Time.fixedDeltaTime;

            bool shouldBeActive = _attackElapsed >= _attackSettings.SimpleAttackHitboxStart
                               && _attackElapsed <  _attackSettings.SimpleAttackHitboxEnd;

            SetHitboxActive(shouldBeActive);

            if (_attackElapsed >= _attackSettings.SimpleAttackDuration)
                EndAttack();
        }

        private void OnSimpleAttack(SimpleAttackAction action)
        {
            if (!_isEnabled || _view == null || _isAttacking)
                return;

            if (action.EventType == InputEventType.Pressed)
                StartAttack();
        }

        private void StartAttack()
        {
            _isAttacking   = true;
            _attackElapsed = 0f;
            _animationController.TriggerPunch();
            _hitController.Enable();
        }

        private void EndAttack()
        {
            SetHitboxActive(false);
            _isAttacking = false;
            _hitController.Disable();
        }

        public void CancelAttack()
        {
            if (_isAttacking)
                EndAttack();
        }

        private void SetHitboxActive(bool active)
        {
            if (_isHitboxActive == active)
                return;

            _view.RightArmCollider.enabled = active;
            _isHitboxActive = active;
        }

        public void Enable()
            => _isEnabled = true;

        public void Disable()
        {
            _isEnabled = false;
            CancelAttack();
        }

        public void Dispose()
            => _inputEvents.Unsubscribe<SimpleAttackAction>(OnSimpleAttack);
    }
}
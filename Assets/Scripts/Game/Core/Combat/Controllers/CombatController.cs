using Game.Common.Input;
using Game.Core.Character;
using Game.Core.Events;
using UnityEngine;

namespace Game.Core.Combat
{
    public class CombatController : ICombatController, System.IDisposable
    {
        private readonly InputEventsBus _inputEventsBus;
        private readonly GameEventsBus  _gameEventsBus;
        private readonly AttackSettings _settings;

        private bool  _isEnabled = true;
        private bool  _isAttacking;
        private bool  _isHitboxActive;
        private float _attackElapsed;

        public CombatController(
            AttackSettings settings,
            GameEventsBus  gameEventsBus,
            InputEventsBus inputEventsBus
        )
        {
            _settings       = settings;
            _inputEventsBus = inputEventsBus;
            _gameEventsBus  = gameEventsBus;
            
            _inputEventsBus.Subscribe<SimpleAttackAction>(OnSimpleAttack);
        }

        public void FixedTick()
        {
            if (!_isAttacking)
                return;

            _attackElapsed += Time.fixedDeltaTime;

            bool shouldBeActive = _attackElapsed >= _settings.SimpleAttackHitboxStart
                               && _attackElapsed <  _settings.SimpleAttackHitboxEnd;

            SetHitboxActive(shouldBeActive);

            if (_attackElapsed >= _settings.SimpleAttackDuration)
                EndAttack();
        }

        private void OnSimpleAttack(SimpleAttackAction action)
        {
            if (!_isEnabled || _isAttacking)
                return;

            if (action.EventType == InputEventType.Pressed)
                StartAttack();
        }

        private void StartAttack()
        {
            _isAttacking   = true;
            _attackElapsed = 0f;

            _gameEventsBus.Publish(new OnPunchStartedEvent());
        }

        private void EndAttack()
        {
            SetHitboxActive(false);
            _isAttacking = false;

            _gameEventsBus.Publish(new OnPunchEndedEvent());
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

            _isHitboxActive = active;
            _gameEventsBus.Publish(new OnHitBoxEnabledEvent(active));
        }

        public void Enable()
            => _isEnabled = true;

        public void Disable()
        {
            _isEnabled = false;
            CancelAttack();
        }

        public void Dispose()
            => _inputEventsBus.Unsubscribe<SimpleAttackAction>(OnSimpleAttack);
    }
}
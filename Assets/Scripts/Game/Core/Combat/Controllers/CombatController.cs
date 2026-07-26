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
        private bool  _isSimpleAttack;
        private bool  _isPowerAttack;
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
            _inputEventsBus.Subscribe<PowerAttackAction>(OnPowerAttack);
        }

        public void FixedTick()
        {
            if (_isSimpleAttack)
                HandleSimpleAttack();

            if (_isPowerAttack)
                HandlePowerAttack();
        }

        private void StartSimpleAttack()
        {
            _isSimpleAttack = true;
            _attackElapsed  = 0f;

            _gameEventsBus.Publish(new OnPunchStartedEvent());
        }

        private void EndSimpleAttack()
        {
            SetHitboxActive(false);
            _isSimpleAttack = false;

            _gameEventsBus.Publish(new OnPunchEndedEvent());
        }

        private void HandleSimpleAttack()
        {
            _attackElapsed += Time.fixedDeltaTime;

            bool shouldBeActive = _attackElapsed >= _settings.SimpleAttackHitboxStart
                               && _attackElapsed <  _settings.SimpleAttackHitboxEnd;

            SetHitboxActive(shouldBeActive);

            if (_attackElapsed >= _settings.SimpleAttackDuration)
                EndSimpleAttack();
        }

        private void OnSimpleAttack(SimpleAttackAction action)
        {
            if (!_isEnabled || _isSimpleAttack || _isPowerAttack)
                return;

            if (action.EventType == InputEventType.Pressed)
                StartSimpleAttack();
        }

        private void StartPowerAttack()
        {
            _isPowerAttack = true;
            _attackElapsed   = 0f;

            _gameEventsBus.Publish(new OnPowerPunchStartedEvent());
        }

        private void EndPowerAttack()
        {
            _isPowerAttack = false;
            _gameEventsBus.Publish(new OnPowerPunchEndedEvent());
        }

        private void HandlePowerAttack()
        {
            _attackElapsed += Time.fixedDeltaTime;

            if (_attackElapsed >= _settings.PoweredAttackDuration)
                EndPowerAttack();
        }

        private void OnPowerAttack(PowerAttackAction action)
        {
            if (!_isEnabled || _isSimpleAttack || _isPowerAttack)
                return;

            if (action.EventType == InputEventType.Pressed)
                StartPowerAttack();
        }

        public void CancelAttack()
        {
            if (_isSimpleAttack)
                EndSimpleAttack();

            if (_isPowerAttack)
                EndPowerAttack();
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
using System.Collections.Generic;
using Game.Common.Input;
using Game.Core.GameEvents;
using Game.Core.Systems;
using UnityEngine;

namespace Game.Core.Character
{
    public sealed class CharacterController : ICharacterController
    {
        private readonly ICharacterModel   _model;
        private readonly InputController   _inputController;
        private readonly InputEventsBus    _inputEventsBus;
        private readonly GameEventsBus     _gameEventsBus;
        private readonly ILookSystem       _lookSystem;
        private readonly IAimSystem        _aimSystem;

        private bool _isAiming;
        private bool _isMoveInputActive;

        public CharacterController(
            ICharacterModel           model,
            IEnumerable<IInputSource> inputSources,
            InputEventsBus            inputEventsBus,
            GameEventsBus             gameEventsBus
        )
        {
            _model         = model;
            _gameEventsBus = gameEventsBus;

            _inputEventsBus = inputEventsBus;
            _inputEventsBus.Subscribe<MoveInput>(HandleMove);
            _inputEventsBus.Subscribe<LookInput>(HandleLook);
            _inputEventsBus.Subscribe<JumpAction>(HandleJump);
            _inputEventsBus.Subscribe<SimpleAttackAction>(HandleSimpleAttack);
            _inputEventsBus.Subscribe<PowerAttackAction>(HandlePowerAttack);

            _inputController = new InputController(
                inputSources, inputEventsBus);

            _lookSystem = new LookSystem(
                model.Transform, Camera.main.transform);

            _aimSystem = new AimSystem(model);
        }

        private void HandleMove(MoveInput input)
        {
            _isMoveInputActive = input.IsActive;

            _gameEventsBus.Publish(new OnMoveEvent(
                _model.CharacterID,
                GetMoveDirection(input)
            ));
        }

        private Vector3 GetMoveDirection(MoveInput input)
        {
            if (!input.IsActive)
                return Vector3.zero;

            return _isAiming ? _model.Forward : input.Direction;
        }

        private void HandleLook(LookInput input)
        {
            if (!_isAiming)
                return;

            _aimSystem.Rotate(input.Delta);

            if (_isMoveInputActive)
            {
                _gameEventsBus.Publish(new OnMoveEvent(
                    _model.CharacterID, _model.Forward));
            }
        }

        private void HandleJump(JumpAction action)
        {
            if (action.EventType != InputEventType.Pressed)
                return;

            _gameEventsBus.Publish(new OnJumpEvent(
                _model.CharacterID));
        }

        private void HandleSimpleAttack(SimpleAttackAction action)
        {
            if (action.EventType == InputEventType.Held)
                return;

            _gameEventsBus.Publish(new OnSimpleAttackInputEvent(
                _model.CharacterID, action.EventType == InputEventType.Pressed));
        }

        private void HandlePowerAttack(PowerAttackAction action)
        {
            if (action.EventType == InputEventType.Pressed)
            {
                _isAiming = true;

                if (_isMoveInputActive)
                {
                    _gameEventsBus.Publish(new OnMoveEvent(
                        _model.CharacterID, _model.Forward));
                }
            }
            else if (action.EventType == InputEventType.Released)
            {
                _isAiming = false;

                _gameEventsBus.Publish(
                    new OnPowerAttackRequestedEvent(_model.CharacterID));
            }

            _model.SetAimEnabled(_isAiming);
        }

        public void LateTick()
        {
            if (_isAiming)
                _aimSystem.Update();

            _lookSystem.LateTick();
        }

        public void Dispose()
        {
            _inputEventsBus.Unsubscribe<MoveInput>(HandleMove);
            _inputEventsBus.Unsubscribe<LookInput>(HandleLook);
            _inputEventsBus.Unsubscribe<JumpAction>(HandleJump);
            _inputEventsBus.Unsubscribe<SimpleAttackAction>(HandleSimpleAttack);
            _inputEventsBus.Unsubscribe<PowerAttackAction>(HandlePowerAttack);
        }
    }
}

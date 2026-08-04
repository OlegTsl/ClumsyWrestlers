using System.Collections.Generic;
using Game.Common.Input;
using Game.Core.GameEvents;
using Game.Core.Systems;
using UnityEngine;

namespace Game.Core.Character
{
    public sealed class CharacterController : ICharacterController
    {
        private readonly ICharacterContext _context;
        private readonly ICharacterModel   _model;
        private readonly InputController   _inputController;
        private readonly InputEventsBus    _inputEventsBus;
        private readonly GameEventsBus     _gameEventsBus;
        private readonly ILookSystem       _lookSystem;

        public CharacterController(
            ICharacterContext         context,
            ICharacterModel           model,
            IEnumerable<IInputSource> inputSources,
            InputEventsBus            inputEventsBus,
            GameEventsBus             gameEventsBus
        )
        {
            _model = model;
            _model.OnHitTrigger += OnHit;

            _context       = context;
            _gameEventsBus = gameEventsBus;

            _inputEventsBus = inputEventsBus;
            _inputEventsBus.Subscribe<MoveInput>(HandleMove);
            _inputEventsBus.Subscribe<JumpAction>(HandleJump);
            _inputEventsBus.Subscribe<SimpleAttackAction>(HandleSimpleAttack);
            _inputEventsBus.Subscribe<PowerAttackAction>(HandlePowerAttack);

            _inputController = new InputController(
                inputSources, inputEventsBus);

            _lookSystem = new LookSystem(inputEventsBus,
                model.Transform, model.CameraTransform);
        }

        private void OnHit(Collider other)
        {
            var target = _context.GetModel(other);
            if (target == null)
                return;

            _gameEventsBus.Publish(new OnDamageEvent(
                _model.CharacterID, target.CharacterID
            ));
        }

        private void HandleMove(MoveInput input)
        {
            _gameEventsBus.Publish(new OnMoveEvent(
                _model.CharacterID,
                input.Direction
            ));
        }

        private void HandleJump(JumpAction action)
        {
            if (action.EventType == InputEventType.Pressed)
            {
                _gameEventsBus.Publish(new OnJumpEvent(
                    _model.CharacterID
                ));
            }
        }

        private void HandleSimpleAttack(SimpleAttackAction action)
        {
            _gameEventsBus.Publish(new OnSimpleAttackEvent(
                _model.CharacterID
            ));
        }

        private void HandlePowerAttack(PowerAttackAction action)
        {
            _gameEventsBus.Publish(new OnPowerAttackEvent(
                _model.CharacterID
            ));
        }

        public void LateTick()
        {
            _lookSystem.LateTick();
        }

        public void Dispose()
        {
            _inputEventsBus.Unsubscribe<MoveInput>(HandleMove);
            _inputEventsBus.Unsubscribe<JumpAction>(HandleJump);
            _inputEventsBus.Unsubscribe<SimpleAttackAction>(HandleSimpleAttack);
            _inputEventsBus.Unsubscribe<PowerAttackAction>(HandlePowerAttack);

            _lookSystem.Dispose();
            _model.OnHitTrigger -= OnHit;
        }
    }
}
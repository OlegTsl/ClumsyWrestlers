using System.Collections.Generic;
using Game.Common.Input;
using Game.Core.GameEvents;
using Game.Core.Systems;

namespace Game.Core.Character
{
    public sealed class CharacterController : ICharacterController
    {
        private readonly ICharacter      _character;
        private readonly InputController _inputController;
        private readonly InputEventsBus  _inputEventsBus;
        private readonly GameEventsBus   _gameEventsBus;
        private readonly ILookSystem     _lookSystem;

        public CharacterController(
            ICharacter                character,
            IEnumerable<IInputSource> inputSources,
            InputEventsBus            inputEventsBus,
            GameEventsBus             gameEventsBus
        )
        {
            _character     = character;
            _gameEventsBus = gameEventsBus;

            _inputEventsBus = inputEventsBus;
            _inputEventsBus.Subscribe<MoveInput>(HandleMove);
            _inputEventsBus.Subscribe<JumpAction>(HandleJump);
            _inputEventsBus.Subscribe<SimpleAttackAction>(HandleSimpleAttack);
            _inputEventsBus.Subscribe<PowerAttackAction>(HandlePowerAttack);

            _inputController = new InputController(
                inputSources, inputEventsBus);

            //_lookSystem = new LookSystem()
        }

        private void HandleMove(MoveInput input)
        {
            _gameEventsBus.Publish(new OnMoveEvent(
                _character.CharacterID,
                input.Direction
            ));
        }

        private void HandleJump(JumpAction action)
        {
            if (action.EventType == InputEventType.Pressed)
            {
                _gameEventsBus.Publish(new OnJumpEvent(
                    _character.CharacterID
                ));
            }
        }

        private void HandleSimpleAttack(SimpleAttackAction action)
        {
            _gameEventsBus.Publish(new OnSimpleAttackEvent(
                _character.CharacterID
            ));
        }

        private void HandlePowerAttack(PowerAttackAction action)
        {
            _gameEventsBus.Publish(new OnPowerAttackEvent(
                _character.CharacterID
            ));
        }

        public void Dispose()
        {
            _inputEventsBus.Unsubscribe<MoveInput>(HandleMove);
            _inputEventsBus.Unsubscribe<JumpAction>(HandleJump);
            _inputEventsBus.Unsubscribe<SimpleAttackAction>(HandleSimpleAttack);
            _inputEventsBus.Unsubscribe<PowerAttackAction>(HandlePowerAttack);
        }
    }
}
using Game.Common.Input;
using Game.Core.Commands;
using Game.Core.Systems;
using UnityEngine;

namespace Game.Core.Character
{
    public sealed class CharacterController : ICharacterController
    {
        private readonly ICharacterModel       _model;
        private readonly IInputEventsBus       _inputEvents;
        private readonly ICharacterCommandSink _commandSink;
        private readonly ISimulationClock      _clock;
        private readonly ILookSystem           _lookSystem;

        public CharacterController(
            ICharacterModel       model,
            ICharacterView        view,
            IInputEventsBus       inputEvents,
            ICharacterCommandSink commandSink,
            ISimulationClock      clock,
            Transform             camera
        )
        {
            _model       = model;
            _inputEvents = inputEvents;
            _commandSink = commandSink;
            _clock       = clock;

            _inputEvents.Subscribe<MoveInput>(HandleMove);
            _inputEvents.Subscribe<JumpAction>(HandleJump);
            _inputEvents.Subscribe<SimpleAttackAction>(HandleSimpleAttack);
            _inputEvents.Subscribe<PowerAttackAction>(HandlePowerAttack);

            _lookSystem = new LookSystem(view.Transform, camera);
        }

        private void HandleMove(MoveInput input)
            => Enqueue(new CharacterCommand(_model.CharacterID, _clock.NextTick,
                CharacterCommandType.Move, direction: input.Direction));

        private void HandleJump(JumpAction action)
        {
            if (action.EventType == InputEventType.Pressed)
                EnqueueAction(CharacterCommandType.Jump, action.EventType);
        }

        private void HandleSimpleAttack(SimpleAttackAction action)
        {
            if (action.EventType != InputEventType.Held)
                EnqueueAction(CharacterCommandType.SimpleAttack, action.EventType);
        }

        private void HandlePowerAttack(PowerAttackAction action)
        {
            if (action.EventType != InputEventType.Held)
                EnqueueAction(CharacterCommandType.PowerAttack, action.EventType);
        }

        private void EnqueueAction(CharacterCommandType type, InputEventType inputType)
            => Enqueue(new CharacterCommand(_model.CharacterID, _clock.NextTick,
                type, inputEventType: inputType));

        private void Enqueue(in CharacterCommand command)
            => _commandSink.TryEnqueue(command);

        public void LateTick()
        {
            _lookSystem.LateTick();
        }

        public void Dispose()
        {
            _inputEvents.Unsubscribe<MoveInput>(HandleMove);
            _inputEvents.Unsubscribe<JumpAction>(HandleJump);
            _inputEvents.Unsubscribe<SimpleAttackAction>(HandleSimpleAttack);
            _inputEvents.Unsubscribe<PowerAttackAction>(HandlePowerAttack);

            _lookSystem.Dispose();
        }
    }
}

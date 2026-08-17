using Game.Common.Input;
using Game.Core.Commands;
using Game.Core.Systems;
using UnityEngine;

namespace Game.Core.Character
{
    public sealed class CharacterController : ICharacterController
    {
        private readonly ICharacterModel _model;
        private readonly ICharacterAimState _aimState;
        private readonly IInputEventsBus _inputEvents;
        private readonly ICharacterCommandSink _commandSink;
        private readonly ISimulationClock _clock;
        private readonly ILookSystem _lookSystem;
        private readonly IAimSystem _aimSystem;

        public CharacterController(
            ICharacterModel model,
            ICharacterView view,
            IInputEventsBus inputEvents,
            ICharacterCommandSink commandSink,
            ISimulationClock clock,
            Transform camera
        )
        {
            _model = model;
            _aimState = model.GetState<ICharacterAimState>();
            _inputEvents = inputEvents;
            _commandSink = commandSink;
            _clock = clock;

            _inputEvents.Subscribe<MoveInput>(HandleMove);
            _inputEvents.Subscribe<LookInput>(HandleLook);
            _inputEvents.Subscribe<JumpAction>(HandleJump);
            _inputEvents.Subscribe<SimpleAttackAction>(HandleSimpleAttack);
            _inputEvents.Subscribe<PowerAttackAction>(HandlePowerAttack);

            _lookSystem = new LookSystem(view.Transform, camera);
            _aimSystem = new AimSystem(
                model.GetState<ICharacterTransformState>(),
                view);
        }

        private void HandleMove(MoveInput input)
            => Enqueue(new CharacterCommand(
                _model.CharacterID,
                _clock.NextTick,
                CharacterCommandType.Move,
                direction: input.Direction));

        private void HandleLook(LookInput input)
        {
            if (input.IsActive)
            {
                Enqueue(new CharacterCommand(
                    _model.CharacterID,
                    _clock.NextTick,
                    CharacterCommandType.Look,
                    lookDelta: input.Delta));
            }
        }

        private void HandleJump(JumpAction action)
        {
            if (action.EventType == InputEventType.Pressed)
            {
                EnqueueAction(CharacterCommandType.Jump, action.EventType);
            }
        }

        private void HandleSimpleAttack(SimpleAttackAction action)
        {
            if (action.EventType != InputEventType.Held)
            {
                EnqueueAction(CharacterCommandType.SimpleAttack, action.EventType);
            }
        }

        private void HandlePowerAttack(PowerAttackAction action)
        {
            if (action.EventType != InputEventType.Held)
            {
                EnqueueAction(CharacterCommandType.PowerAttack, action.EventType);
            }
        }

        private void EnqueueAction(CharacterCommandType type, InputEventType inputType)
            => Enqueue(new CharacterCommand(
                _model.CharacterID,
                _clock.NextTick,
                type,
                inputEventType: inputType));

        private void Enqueue(in CharacterCommand command)
            => _commandSink.TryEnqueue(command);

        public void LateTick()
        {
            if (_aimState.IsAiming)
            {
                _aimSystem.Update();
            }

            _lookSystem.LateTick();
        }

        public void Dispose()
        {
            _inputEvents.Unsubscribe<MoveInput>(HandleMove);
            _inputEvents.Unsubscribe<LookInput>(HandleLook);
            _inputEvents.Unsubscribe<JumpAction>(HandleJump);
            _inputEvents.Unsubscribe<SimpleAttackAction>(HandleSimpleAttack);
            _inputEvents.Unsubscribe<PowerAttackAction>(HandlePowerAttack);
            _aimSystem.Dispose();
            _lookSystem.Dispose();
        }
    }
}

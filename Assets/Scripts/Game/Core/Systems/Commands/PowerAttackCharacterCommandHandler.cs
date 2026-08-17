using Game.Common.Input;
using Game.Core.Character;
using Game.Core.Commands;
using Game.Core.GameEvents;

namespace Game.Core.Systems
{
    public sealed class PowerAttackCharacterCommandHandler :
        ICharacterCommandHandler
    {
        private readonly ICharacterContext _characters;
        private readonly IGameEventsBus _events;
        private readonly CharacterControlStateRegistry _states;
        private readonly CharacterMovementCommandPublisher _movementPublisher;

        public CharacterCommandType CommandType
            => CharacterCommandType.PowerAttack;

        public PowerAttackCharacterCommandHandler(
            ICharacterContext characters,
            IGameEventsBus events,
            CharacterControlStateRegistry states,
            CharacterMovementCommandPublisher movementPublisher
        )
        {
            _characters = characters;
            _events = events;
            _states = states;
            _movementPublisher = movementPublisher;
        }

        public void Handle(in CharacterCommand command)
        {
            ICharacterModel model = _characters.GetModel(command.CharacterId);
            if (model == null ||
                !model.GetState<ICharacterActivityState>().Enabled ||
                !_states.TryGet(command.CharacterId, out CharacterControlState state))
            {
                return;
            }

            if (command.InputEventType == InputEventType.Pressed)
            {
                SetAiming(model, state, true);
            }
            else if (command.InputEventType == InputEventType.Released)
            {
                SetAiming(model, state, false);
                _events.Publish(new OnPowerAttackRequestedEvent(
                    model.CharacterID));
            }
        }

        private void SetAiming(
            ICharacterModel model,
            CharacterControlState state,
            bool isAiming)
        {
            model.GetState<ICharacterAimState>().SetAiming(isAiming);
            _events.Publish(new OnAimStateChangedEvent(
                model.CharacterID,
                isAiming));
            _movementPublisher.Publish(model, state);
        }
    }
}

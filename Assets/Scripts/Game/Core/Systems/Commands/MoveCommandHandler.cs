using Game.Core.Character;
using Game.Core.Commands;
using Game.Core.Extension;

namespace Game.Core.Systems
{
    public sealed class MoveCommandHandler : ICommandHandler
    {
        private readonly ICharacterContext        _characters;
        private readonly ControlStateRegistry     _states;
        private readonly MovementCommandPublisher _movementPublisher;

        public CharacterCommandType CommandType => CharacterCommandType.Move;

        public MoveCommandHandler(
            ICharacterContext        characters,
            ControlStateRegistry     states,
            MovementCommandPublisher movementPublisher
        )
        {
            _characters        = characters;
            _states            = states;
            _movementPublisher = movementPublisher;
        }

        public void Handle(in CharacterCommand command)
        {
            ICharacterModel model = _characters.GetModel(command.CharacterId);
            
            if (model == null || !model.GetState<ICharacterActivityState>().Enabled ||
                !_states.TryGet(command.CharacterId, out CharacterControlState state))
            {
                return;
            }

            state.MoveDirection = command.Direction;
            _movementPublisher.Publish(model, state);
        }
    }
}

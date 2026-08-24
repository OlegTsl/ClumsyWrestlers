using Game.Common.Input;
using Game.Core.Character;
using Game.Core.Commands;
using Game.Core.Extension;
using Game.Core.GameEvents;

namespace Game.Core.Systems
{
    public sealed class JumpCommandHandler : ICommandHandler
    {
        private readonly ICharacterContext _characters;
        private readonly IGameEventsBus    _events;

        public CharacterCommandType CommandType => CharacterCommandType.Jump;

        public JumpCommandHandler(
            ICharacterContext characters,
            IGameEventsBus    events
        )
        {
            _characters = characters;
            _events     = events;
        }

        public void Handle(in CharacterCommand command)
        {
            if (command.InputEventType != InputEventType.Pressed)
            {
                return;
            }

            ICharacterModel model = _characters.GetModel(command.CharacterId);
            if (model == null)
            {
                return;
            }

            ICharacterMovementRuntimeState movement =
                model.GetState<ICharacterMovementRuntimeState>();
            if (!movement.Enabled ||
                !movement.IsMovable ||
                !movement.IsGrounded ||
                movement.Velocity.y > 0f)
            {
                return;
            }

            _events.Publish(new OnJumpEvent(model.CharacterID));
        }
    }
}

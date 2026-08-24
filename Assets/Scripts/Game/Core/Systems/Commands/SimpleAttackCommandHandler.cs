using Game.Common.Input;
using Game.Core.Character;
using Game.Core.Commands;
using Game.Core.Extension;
using Game.Core.GameEvents;

namespace Game.Core.Systems
{
    public sealed class SimpleAttackCommandHandler : ICommandHandler
    {
        private readonly ICharacterContext _characters;
        private readonly IGameEventsBus _events;

        public CharacterCommandType CommandType
            => CharacterCommandType.SimpleAttack;

        public SimpleAttackCommandHandler(
            ICharacterContext characters,
            IGameEventsBus    events
        )
        {
            _characters = characters;
            _events     = events;
        }

        public void Handle(in CharacterCommand command)
        {
            if (command.InputEventType == InputEventType.Held)
                return;

            ICharacterModel model = _characters.GetModel(command.CharacterId);
            if (model != null && model.GetState<ICharacterActivityState>().Enabled)
            {
                _events.Publish(new OnSimpleAttackInputEvent(
                    model.CharacterID, command.InputEventType == InputEventType.Pressed));
            }
        }
    }
}

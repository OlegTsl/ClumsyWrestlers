using Game.Common.Input;
using Game.Core.Character;
using Game.Core.Commands;
using Game.Core.Extension;
using Game.Core.GameEvents;

namespace Game.Core.Systems
{
    public sealed class BlockCommandHandler : ICommandHandler
    {
        private readonly ICharacterContext _characters;
        private readonly IGameEventsBus    _events;

        public CharacterCommandType CommandType => CharacterCommandType.Block;

        public BlockCommandHandler(
            ICharacterContext characters,
            IGameEventsBus    events
        )
        {
            _characters = characters;
            _events     = events;
        }

        public void Handle(in CharacterCommand command)
        {
            ICharacterModel model = _characters.GetModel(command.CharacterId);
            if (model != null && model.GetState<ICharacterActivityState>().Enabled)
            {
                _events.Publish(new OnBlockEvent(model.CharacterID,
                    command.InputEventType == InputEventType.Pressed));
            }
        }
    }
}

using Game.Core.Commands;

namespace Game.Core.Systems
{
    public interface ICharacterCommandHandler
    {
        CharacterCommandType CommandType { get; }
        void Handle(in CharacterCommand command);
    }
}

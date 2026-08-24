using Game.Core.Commands;

namespace Game.Core.Systems
{
    public interface ICommandHandler
    {
        CharacterCommandType CommandType { get; }
        void Handle(in CharacterCommand command);
    }
}

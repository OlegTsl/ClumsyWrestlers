namespace Game.Core.Commands
{
    public interface ICharacterCommandSink
    {
        bool TryEnqueue(in CharacterCommand command);
    }
}

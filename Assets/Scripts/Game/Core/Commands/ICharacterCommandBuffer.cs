namespace Game.Core.Commands
{
    public interface ICharacterCommandBuffer : ICharacterCommandSink
    {
        int Count { get; }
        bool TryDequeueDue(uint currentTick, out CharacterCommand command);
        void Clear();
    }
}

using Game.Core.Commands;

namespace Game.Core.GameEvents
{
    public readonly struct OnCharacterCommandEvent
    {
        public CharacterCommand Command { get; }

        public OnCharacterCommandEvent(CharacterCommand command)
            => Command = command;
    }
}

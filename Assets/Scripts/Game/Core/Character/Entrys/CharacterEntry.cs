using System;
using Game.Common.Input;

namespace Game.Core.Character
{
    public sealed class CharacterEntry
    {
        public readonly ICharacterContext  Context;
        public readonly Action<MoveInput>  OnMove;
        public readonly Action<JumpAction> OnJump;

        public bool IsEnabled = true;
        public bool HasMoveInput;

        public CharacterEntry(
            ICharacterContext  context,
            Action<MoveInput>  onMove,
            Action<JumpAction> onJump)
        {
            Context = context;
            OnMove  = onMove;
            OnJump  = onJump;
        }
    }
}
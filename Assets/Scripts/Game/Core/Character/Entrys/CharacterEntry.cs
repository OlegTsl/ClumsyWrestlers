using System;
using Game.Common.Input;
using Game.Core.Animation;
using Game.Core.Combat;
using Game.Core.Movement;

namespace Game.Core.Character
{
    public sealed class CharacterEntry
    {
        public readonly ICharacterContext     Context;
        public readonly IMovementController   Movement;
        public readonly ICombatController     Combat;
        public readonly IAnimationController  Animation;
        public readonly ILookController       Look;
        public readonly InputController       Input;

        public readonly Action<MoveInput>  OnMove;
        public readonly Action<JumpAction> OnJump;

        public bool IsEnabled = true;
        public bool HasMoveInput;

        public CharacterEntry(ICharacterContext context, Action<MoveInput> onMove, Action<JumpAction> onJump)
        {
            Context   = context;
            Movement  = context.GetSystem<IMovementController>();
            Combat    = context.GetSystem<ICombatController>();
            Animation = context.GetSystem<IAnimationController>();
            Input     = context.GetSystem<InputController>();
            Look      = context.TryGetSystem<ILookController>(out var look) ? look : null;

            OnMove = onMove;
            OnJump = onJump;
        }
    }
}
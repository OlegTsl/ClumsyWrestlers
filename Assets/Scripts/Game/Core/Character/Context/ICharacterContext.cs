using System;
using Game.Common.Input;
using Game.Core.Animation;
using Game.Core.Combat;
using Game.Core.Events;
using Game.Core.Movement;

namespace Game.Core.Character
{
    public interface ICharacterContext
    {
        ICharacterView       View        { get; }
        IAnimationController Animation   { get; }
        IMovementController  Movement    { get; }
        IDamageController    Damage      { get; }
        ICombatController    Combat      { get; }
        ILookController      Look        { get; }
        IHitController       Hit         { get; }
        InputController      Input       { get; }
        InputEventsBus       InputEvents { get; }
        GameEventsBus        GameEvents  { get; }
        Guid                 CharacterId { get; }
    }
}
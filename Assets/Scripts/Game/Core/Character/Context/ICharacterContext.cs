using System;
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
        IHitController       Hit         { get; }
        GameEventsBus        GameEvents  { get; }
        Guid                 CharacterId { get; }
    }
}
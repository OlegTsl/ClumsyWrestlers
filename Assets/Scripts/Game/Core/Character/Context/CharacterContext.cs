using System;
using Game.Common.Input;
using Game.Core.Animation;
using Game.Core.Combat;
using Game.Core.Events;
using Game.Core.Movement;

namespace Game.Core.Character
{
    public class CharacterContext : ICharacterContext
    {
        public ICharacterView       View        { get; set; }
        public IAnimationController Animation   { get; set; }
        public IMovementController  Movement    { get; set; }
        public IDamageController    Damage      { get; set; }
        public ICombatController    Combat      { get; set; }
        public ILookController      Look        { get; set; }
        public IHitController       Hit         { get; set; }
        public InputController      Input       { get; set; }
        public InputEventsBus       InputEvents { get; set; }
        public GameEventsBus        GameEvents  { get; set; }
        public Guid                 CharacterId { get; set; }
    }
}
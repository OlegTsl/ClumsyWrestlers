using System.Collections.Generic;
using Game.Common.Views;
using Game.Core.Components;
using UnityEngine;

namespace Game.Core.Character
{
    public interface ICharacterView : IView
    {
        CharacterData           Data             { get; }
        Rigidbody               Rigidbody        { get; }
        Animator                Animator         { get; }
        Collider                HitBox           { get; }
        Collider                LeftArmCollider  { get; }
        Collider                RightArmCollider { get; }
        Collider                LeftLegCollider  { get; }
        Collider                RightLegCollider { get; }
        Transform               Transform        { get; }
        Camera                  CharacterCamera  { get; }
        ColliderHandler         ColliderHandler  { get; }
        IReadOnlyList<Collider> AttackColliders  { get; }

        void SetAsPlayer(bool isPlayer);
    }
}
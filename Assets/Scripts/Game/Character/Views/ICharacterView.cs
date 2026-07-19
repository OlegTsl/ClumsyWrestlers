using Game.Common.Views;
using UnityEngine;

namespace Game.Character
{
    public interface ICharacterView : IView
    {
        CharacterData Data           { get; }
        Rigidbody Rigidbody          { get; }
        Animator Animator            { get; }
        CapsuleCollider BodyCollider { get; }
        Collider[] AttackColliders   { get; }
        Transform Transform          { get; }
    }
}
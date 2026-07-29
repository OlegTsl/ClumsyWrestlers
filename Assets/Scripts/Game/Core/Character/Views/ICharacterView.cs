using System.Collections.Generic;
using Game.Common.Views;
using Game.Core.Components;
using UnityEngine;

namespace Game.Core.Character
{
    public interface ICharacterView : IView
    {
        CharacterData           Data             { get; }
        Collider                Hitbox           { get; }
        Collider                LeftArmCollider  { get; }
        Collider                RightArmCollider { get; }
        Collider                LeftLegCollider  { get; }
        Collider                RightLegCollider { get; }
        Camera                  CharacterCamera  { get; }
        ColliderHandler         ColliderHandler  { get; }
        IReadOnlyList<Collider> AttackColliders  { get; }

        void SetAsPlayer(bool isPlayer);
        void SetVelocity(Vector3 velocity);
        void SetPosition(Vector3 position);
        void SetRotation(Quaternion rotation);

        void SetAnimatorFloat(int id, float value, float dampTime, float deltaTime);
        void SetAnimatorBool(int id, bool value);
        void SetAnimatorTrigger(int id);

        bool IsGrounded();
        bool IsMoving();

        Vector3 TransformDirection(Vector3 direction);
        Vector3 InverseTransformDirection(Vector3 direction);
        Vector3 GetVelocity();
    }
}
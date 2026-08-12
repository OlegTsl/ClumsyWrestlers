using System;
using System.Collections.Generic;
using Game.Common.Views;
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
        IReadOnlyList<Collider> AttackColliders  { get; }
        Transform               Transform        { get; }
        LineRenderer            Aim              { get; }

        void SetVelocity(Vector3 velocity);
        void SetPosition(Vector3 position);
        Vector3 GetPosition();
        void SetRotation(Quaternion rotation);

        void SetAnimatorFloat(int id, float value, float dampTime, float deltaTime);
        void SetAnimatorBool(int id, bool value);
        void SetAnimatorTrigger(int id);
        void SetAimPositions(Vector3[] positions);
        void SetAimPositionCount(int count);

        bool IsGrounded();
        bool IsMoving();

        Vector3 TransformDirection(Vector3 direction);
        Vector3 InverseTransformDirection(Vector3 direction);
        Vector3 GetVelocity();
    }
}
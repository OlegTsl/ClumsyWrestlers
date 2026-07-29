using System;
using UnityEngine;

namespace Game.Core.Character
{
    public interface ICharacter
    {
        void SetPosition(Vector3 position);
        void SetRotation(Quaternion rotation);
        void SetEnabled(bool enabled);
        void SetAnimatorFloat(int id, float value, float dampTime, float deltaTime);
        void SetAnimatorBool(int id, bool value);
        void SetAnimatorTrigger(int id);

        Guid          CharacterID { get; }
        CharacterData Data        { get; }
        Collider      Hitbox      { get; }
        bool          Enabled     { get; }

        bool IsGrounded();
        bool IsMoving();

        void ApplyVelocity(Vector3 velocity);

        Vector3 TransformDirection(Vector3 direction);
        Vector3 InverseTransformDirection(Vector3 direction);

        Vector3 GetVelocity();
    }
}
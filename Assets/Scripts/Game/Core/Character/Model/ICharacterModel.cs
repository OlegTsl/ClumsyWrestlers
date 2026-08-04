using System;
using UnityEngine;

namespace Game.Core.Character
{
    public interface ICharacterModel : IDisposable
    {
        void SetPosition(Vector3 position);
        void SetRotation(Quaternion rotation);
        void SetEnabled(bool enabled);
        void SetAnimatorFloat(int id, float value, float dampTime, float deltaTime);
        void SetAnimatorBool(int id, bool value);
        void SetAnimatorTrigger(int id);
        void ApplyVelocity(Vector3 velocity);
        void SetHitsEnabled(bool enabled);
        void SetCameraEnabled(bool enabled);

        Guid          CharacterID     { get; }
        CharacterData Data            { get; }
        Collider      Hitbox          { get; }
        Transform     Transform       { get; }
        Transform     CameraTransform { get; }
        bool          Enabled         { get; }

        bool IsGrounded();
        bool IsMoving();

        Vector3 TransformDirection(Vector3 direction);
        Vector3 InverseTransformDirection(Vector3 direction);
        Vector3 GetVelocity();

        event Action<Collider> OnHitTrigger;
    }
}
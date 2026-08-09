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
        void SetAimEnabled(bool enabled);
        void SetAimPositions(Vector3[] positions);
        void SetAimPositionCount(int count);

        Guid          CharacterID     { get; }
        CharacterData Data            { get; }
        Collider      Hitbox          { get; }
        Transform     Transform       { get; }
        LineRenderer  Aim             { get; }
        Vector3       Forward         { get; }
        Vector3       Position        { get; }
        Quaternion    Rotation        { get; }
        bool          Enabled         { get; }

        bool IsGrounded();
        bool IsMoving();

        Vector3 TransformDirection(Vector3 direction);
        Vector3 InverseTransformDirection(Vector3 direction);
        Vector3 GetVelocity();

        event Action<Collider> OnHitTrigger;
    }
}
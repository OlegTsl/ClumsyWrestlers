using Game.Core.Entities;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Character
{
    public sealed class CharacterModel :
        ICharacterModel,
        ICharacterTransformState,
        ICharacterPhysicsState,
        ICharacterActivityState,
        ICharacterMovementState,
        ICharacterAimState,
        ICharacterMovementRuntimeState,
        ICharacterCombatRuntimeState,
        ICharacterPresentationState
    {
        public EntityId CharacterID { get; }
        public CharacterData Data { get; }
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Vector3 Velocity { get; private set; }
        public Vector3 AttackOrigin { get; private set; }
        public bool Enabled { get; private set; }
        public bool IsMovable { get; private set; } = true;
        public bool IsGrounded { get; private set; }
        public bool IsAiming { get; private set; }

        public Vector3 Forward
            => Rotation * Vector3.forward;

        public CharacterModel(EntityId characterId, CharacterData data)
        {
            CharacterID = characterId;
            Data = data;
            Rotation = Quaternion.identity;
        }

        public void SetPosition(Vector3 position)
            => Position = position;

        public void SetRotation(Quaternion rotation)
            => Rotation = rotation;

        public void SetVelocity(Vector3 velocity)
            => Velocity = velocity;

        public void SetEnabled(bool enabled)
            => Enabled = enabled;

        public void SetMovable(bool isMovable)
            => IsMovable = isMovable;

        public void SetAiming(bool isAiming)
            => IsAiming = isAiming;

        public void SynchronizePhysics(in CharacterPhysicsSnapshot snapshot)
        {
            Position = snapshot.Position;
            Rotation = snapshot.Rotation;
            Velocity = snapshot.Velocity;
            AttackOrigin = snapshot.AttackOrigin;
            IsGrounded = snapshot.IsGrounded;
        }

        public Vector3 InverseTransformDirection(Vector3 direction)
            => Quaternion.Inverse(Rotation) * direction;
    }
}

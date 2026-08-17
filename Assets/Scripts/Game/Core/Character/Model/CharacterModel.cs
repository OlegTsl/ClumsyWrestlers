using Game.Core.Teams;
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
        ICharacterMovementRuntimeState,
        ICharacterCombatRuntimeState,
        ICharacterPresentationState,
        ICharacterTeamState
    {
        public EntityId      CharacterID  { get; }
        public CharacterData Data         { get; }
        public TeamId        TeamId       { get; }
        public Vector3       Position     { get; private set; }
        public Quaternion    Rotation     { get; private set; }
        public Vector3       Velocity     { get; private set; }
        public Vector3       AttackOrigin { get; private set; }
        public bool          Enabled      { get; private set; }
        public bool          IsMovable    { get; private set; } = true;
        public bool          IsGrounded   { get; private set; }

        public Vector3 Forward
            => Rotation * Vector3.forward;

        public CharacterModel(EntityId characterId, CharacterData data, TeamId teamId)
        {
            CharacterID = characterId;
            Data        = data;
            TeamId      = teamId;
            Rotation    = Quaternion.identity;
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

        public void SynchronizePhysics(in CharacterPhysicsSnapshot snapshot)
        {
            Position     = snapshot.Position;
            Rotation     = snapshot.Rotation;
            Velocity     = snapshot.Velocity;
            AttackOrigin = snapshot.AttackOrigin;
            IsGrounded   = snapshot.IsGrounded;
        }

        public Vector3 InverseTransformDirection(Vector3 direction)
            => Quaternion.Inverse(Rotation) * direction;
    }
}

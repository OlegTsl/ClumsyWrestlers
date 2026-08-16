using System;
using UnityEngine;

namespace Game.Core.Environment
{
    public interface IInteractableChestView
    {
        Guid ChestID         { get; }
        EnvironmentData Data { get; }
        Collider Hitbox      { get; }
        Vector3 Position     { get; }

        void ApplyImpulse(Vector3 impulse);
        void DampenAfterImpact(Vector3 impactVelocity);
        bool TryConsumeCollision(out EnvironmentCollisionData collision);
    }
}

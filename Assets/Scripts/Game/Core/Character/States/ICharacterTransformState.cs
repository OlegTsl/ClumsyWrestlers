using UnityEngine;

namespace Game.Core.Character
{
    public interface ICharacterTransformState : ICharacterModel
    {
        Vector3    Position     { get; }
        Quaternion Rotation     { get; }
        Vector3    Forward      { get; }
        Vector3    AttackOrigin { get; }

        void SetPosition(Vector3 position);
        void SetRotation(Quaternion rotation);
    
        Vector3 InverseTransformDirection(Vector3 direction);
    }
}

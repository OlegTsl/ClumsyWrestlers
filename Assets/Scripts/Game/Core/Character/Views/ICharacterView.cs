using Game.Common.Views;
using Game.Core.Bots;
using UnityEngine;

namespace Game.Core.Character
{
    public interface ICharacterView : IView
    {
        CharacterData       Data               { get; }
        Collider            Hitbox             { get; }
        Rigidbody           Rigidbody          { get; }
        Transform           Transform          { get; }
        IBotNavigationAgent BotNavigationAgent { get; }
        bool                IsAlive            { get; }
        int KnockdownAnimatorStateHash          { get; }

        CharacterPhysicsSnapshot CapturePhysicsSnapshot();
        void SetVelocity(Vector3 velocity);
        void SetPosition(Vector3 position);
        void SetRotation(Quaternion rotation);
        void MoveRotation(Quaternion rotation);
        void SetVisualLean(Quaternion rotation);
        void SetAnimatorFloat(int id, float value, float dampTime, float deltaTime);
        void SetAnimatorBool(int id, bool value);
        void SetAnimatorTrigger(int id);
        void ResetAnimatorTrigger(int id);
        void SetAttackHandIk(AttackHand hand, Vector3 position, float weight);
        void ClearAttackHandIk();
    }
}

using UnityEngine;

namespace Game.Core.Environment
{
    [CreateAssetMenu(fileName = "EnvironmentData", menuName = "Game/Environment/Environment Data")]
    public sealed class EnvironmentData : ScriptableObject
    {
        [Header("Chest Impact")]
        [SerializeField, Range(0f, 30f)] private float _hitForceMultiplier = 4f;
         public float HitForceMultiplier => _hitForceMultiplier;

        [SerializeField, Range(0f, 1f)] private float _forceDamping = 0.6f;
        public float ForceDamping => _forceDamping;

        [SerializeField, Range(0f, 30f)] private float _targetImpactForce = 6f;
        public float TargetImpactForce => _targetImpactForce;

        [SerializeField, Range(0f, 5f)] private float _knockbackHeight = 1f;
        public float KnockbackHeight => _knockbackHeight;
    }
}

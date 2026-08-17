using System;
using UnityEngine;

namespace Game.Core.Bots
{
    [CreateAssetMenu(
        fileName = "BotBehaviorProfile",
        menuName = "Game/Bots/Behavior Profile")]
    public sealed class BotBehaviorProfile :
        ScriptableObject,
        IBotBehaviorSettings
    {
        [Header("Decision Making")]
        [SerializeField, Range(0.05f, 1f)] private float _decisionInterval = 0.1f;
        [SerializeField, Min(1f)] private float _perceptionRadius = 20f;
        [SerializeField, Range(0f, 1f)] private float _targetSwitchBias = 0.15f;

        [Header("Navigation")]
        [SerializeField, Min(0.05f)] private float _arrivalDistance = 0.65f;
        [SerializeField, Min(0.1f)] private float _navigationSampleDistance = 2f;
        [SerializeField, Min(0.1f)] private float _edgeRecoveryDistance = 1f;
        [SerializeField, Min(0f)] private float _jumpCooldown = 0.8f;

        [Header("Combat")]
        [SerializeField, Min(0.5f)]
        private float _simpleAttackDistanceMultiplier = 1.15f;
        [SerializeField, Min(0f)] private float _powerAttackMinimumDistance = 2f;
        [SerializeField, Min(0f)] private float _powerAttackMaximumDistance = 4.5f;
        [SerializeField, Min(0f)] private float _attackCooldown = 0.45f;
        [SerializeField, Min(0f)] private float _powerAttackHoldDuration = 0.2f;
        [SerializeField, Range(-1f, 1f)] private float _facingDotThreshold = 0.8f;
        [SerializeField, Range(0.011f, 0.2f)] private float _turnInputMagnitude = 0.03f;

        [Header("Utility Weights")]
        [SerializeField, Min(0f)] private float _chaseWeight = 0.35f;
        [SerializeField, Min(0f)] private float _simpleAttackWeight = 1f;
        [SerializeField, Min(0f)] private float _powerAttackWeight = 0.55f;
        [SerializeField, Min(0f)] private float _environmentWeight = 0.75f;

        [Header("Environment")]
        [SerializeField, Min(0f)] private float _pushSearchRadius = 9f;
        [SerializeField, Min(0f)] private float _pushEnemyMaximumDistance = 4f;
        [SerializeField, Min(0.1f)] private float _pushStandOffDistance = 1f;
        [SerializeField, Min(0.1f)] private float _pushLaneRadius = 1.1f;
        [SerializeField, Range(-1f, 1f)]
        private float _pushRequiredOutwardAlignment = 0.25f;

        public float DecisionInterval => _decisionInterval;
        public float PerceptionRadius => _perceptionRadius;
        public float TargetSwitchBias => _targetSwitchBias;
        public float ArrivalDistance => _arrivalDistance;
        public float NavigationSampleDistance => _navigationSampleDistance;
        public float EdgeRecoveryDistance => _edgeRecoveryDistance;
        public float SimpleAttackDistanceMultiplier
            => _simpleAttackDistanceMultiplier;
        public float PowerAttackMinimumDistance => _powerAttackMinimumDistance;
        public float PowerAttackMaximumDistance => _powerAttackMaximumDistance;
        public float AttackCooldown => _attackCooldown;
        public float PowerAttackHoldDuration => _powerAttackHoldDuration;
        public float ChaseWeight => _chaseWeight;
        public float SimpleAttackWeight => _simpleAttackWeight;
        public float PowerAttackWeight => _powerAttackWeight;
        public float EnvironmentWeight => _environmentWeight;
        public float PushSearchRadius => _pushSearchRadius;
        public float PushEnemyMaximumDistance => _pushEnemyMaximumDistance;
        public float PushStandOffDistance => _pushStandOffDistance;
        public float PushLaneRadius => _pushLaneRadius;
        public float PushRequiredOutwardAlignment
            => _pushRequiredOutwardAlignment;
        public float JumpCooldown => _jumpCooldown;
        public float FacingDotThreshold => _facingDotThreshold;
        public float TurnInputMagnitude => _turnInputMagnitude;

        public void Validate()
        {
            if (_decisionInterval <= 0f ||
                _perceptionRadius <= 0f ||
                _arrivalDistance <= 0f ||
                _navigationSampleDistance <= 0f ||
                _edgeRecoveryDistance <= 0f)
            {
                throw new InvalidOperationException(
                    "Bot timing, perception and navigation distances must be positive.");
            }

            if (_powerAttackMaximumDistance < _powerAttackMinimumDistance)
            {
                throw new InvalidOperationException(
                    "Bot power attack maximum distance must be at least the minimum distance.");
            }
        }
    }
}

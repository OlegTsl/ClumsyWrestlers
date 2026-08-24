using UnityEngine;

namespace Game.Core.Character
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Characters/Character Data")]
    public sealed class CharacterData : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private string _characterName = "str_ortis";
        public string CharacterName => _characterName;

        [SerializeField] private float _characterMass = 80f;
        public float CharacterMass => _characterMass;

        [Header("Movement")]
        [SerializeField] private MovementSettings _movement;
        public MovementSettings Movement => _movement;

        [Header("Combat")]
        [SerializeField] private CombatSettings _combatSettings;
        public CombatSettings Combat => _combatSettings;
    }
    
    [System.Serializable]
    public sealed class MovementSettings
    {
        [Header("Ground Movement")]
        [SerializeField, Range(0f, 20f)] private float _runSpeed = 6f;
        public float RunSpeed => _runSpeed;

        [SerializeField, Range(0f, 100f)] private float _acceleration = 50f;
        public float Acceleration => _acceleration;

        [SerializeField, Range(0f, 100f)] private float _deceleration = 40f;
        public float Deceleration => _deceleration;

        [SerializeField, Range(0f, 900f)] private float _rotationSpeed = 500f;
        public float RotationSpeed => _rotationSpeed;

        [Header("Jump")]
        [SerializeField, Range(0f, 20f)] private float _jumpImpulse = 7f;
        public float JumpImpulse => _jumpImpulse;

        [SerializeField, Range(0f, 5f)] private float _gravityMultiplier = 2f;
        public float GravityMultiplier => _gravityMultiplier;

        [Header("Air Control")]
        [SerializeField, Range(0f, 1f)] private float _airControlFactor = 0.3f;
        public float AirControlFactor => _airControlFactor;

        [SerializeField, Range(0f, 5f)] private float _fallingDelay = 2.5f;
        public float FallingDelay => _fallingDelay;
    }

    [System.Serializable]
    public sealed class CombatSettings
    {
        [Header("Simple Attack")]
        [SerializeField] private SimpleAttackSettings _simpleAttack;
        public SimpleAttackSettings SimpleAttack => _simpleAttack;

        [Header("Power Attack")]
        [SerializeField] private PowerAttackSettings _powerAttack;
        public PowerAttackSettings PowerAttack => _powerAttack;

        [Header("Hit Reaction")]
        [SerializeField] private HitReactionSettings _hitReaction;
        public HitReactionSettings HitReaction => _hitReaction;
    }

    public enum AttackHand
    {
        Left,
        Right
    }

    [System.Serializable]
    public sealed class SimpleAttackSettings
    {
        [Header("Animation")]
        [SerializeField] private AnimationClip _animationClip;

        [SerializeField, Range(0.1f, 5f)] private float _animationSpeed = 1.5f;
        public float AnimationSpeed => Mathf.Max(_animationSpeed, 0.01f);
        public float Duration => _animationClip == null ? 0f : _animationClip.length / AnimationSpeed;

        [Header("Active Window")]
        [SerializeField, Range(0f, 1f)] private float _hitboxStartNormalized = 0.3f;
        public float HitboxStartNormalized => Mathf.Min(_hitboxStartNormalized, _hitboxEndNormalized);
        
        [SerializeField, Range(0f, 1f)] private float _hitboxEndNormalized = 0.5f;
        public float HitboxEndNormalized => Mathf.Max(_hitboxStartNormalized, _hitboxEndNormalized);

        [Header("Hitbox")]
        [SerializeField, Range(0.01f, 5f)] private float _hitboxRange = 0.85f;
        public float HitboxRange => _hitboxRange;

        [SerializeField, Range(0.01f, 2f)] private float _hitboxRadius = 0.3f;
        public float HitboxRadius => _hitboxRadius;

        [Header("Hand IK")]
        [SerializeField, Range(0f, 1f)] private float _handIkWeight = 0.75f;
        public float HandIkWeight => _handIkWeight;

        [SerializeField, Range(0.1f, 1.5f)] private float _handIkReach = 0.65f;
        public float HandIkReach => _handIkReach;

        [Header("Impact")]
        [SerializeField, Range(0f, 50f)] private float _knockbackForce = 5f;
        public float KnockbackForce => _knockbackForce;

        [SerializeField, Range(0f, 5f)] private float _knockbackHeight;
        public float KnockbackHeight => _knockbackHeight;
    }

    [System.Serializable]
    public sealed class PowerAttackSettings
    {
        [Header("Animation")]
        [SerializeField] private AnimationClip _animationClip;
        public AnimationClip AnimationClip => _animationClip;

        [SerializeField, Range(0.1f, 5f)] private float _animationSpeed = 1.5f;
        public float AnimationSpeed => Mathf.Max(_animationSpeed, 0.01f);
        public float Duration => _animationClip == null ? 0f : _animationClip.length / AnimationSpeed;

        [Header("Movement")]
        [SerializeField, Range(0f, 50f)] private float _distance = 6f;
        public float Distance => _distance;

        [SerializeField, Range(0.1f, 50f)] private float _jumpHeight = 1.5f;
        public float JumpHeight => _jumpHeight;

        [Header("Power Wave")]
        [SerializeField, Range(0f, 1f)] private float _waveStartNormalized = 0.625f;
        public float WaveStartNormalized => _waveStartNormalized;
        public float WaveStartTime => Duration * WaveStartNormalized;

        [SerializeField, Range(0f, 20f)] private float _waveRadius = 5f;
        public float WaveRadius => _waveRadius;

        [SerializeField, Range(0f, 5f)] private float _waveHeight = 5f;
        public float WaveHeight => _waveHeight;

        [Header("Impact")]
        [SerializeField, Range(0f, 50f)] private float _knockbackForce = 12f;
        public float KnockbackForce => _knockbackForce;

        [SerializeField, Range(0f, 5f)] private float _knockbackHeight;
        public float KnockbackHeight => _knockbackHeight;
    }

    [System.Serializable]
    public sealed class HitReactionSettings
    {
        [Header("Visual Lean")]
        [SerializeField, Range(0f, 90f)] private float _leanAngle = 12f;
        public float LeanAngle => _leanAngle;

        [SerializeField, Range(0.05f, 1f)] private float _leanDuration = 0.3f;
        public float LeanDuration => Mathf.Max(_leanDuration, 0.01f);
    }
}

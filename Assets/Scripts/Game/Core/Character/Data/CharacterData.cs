using UnityEngine;

namespace Game.Core.Character
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Characters/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private string _characterName = "str_ortis";
        [SerializeField] private float  _characterMass = 80f;
        
        [Header("Movement")]
        [SerializeField] private MovementSettings _movement;
        public MovementSettings Movement => _movement;

        [Header("Combat")]
        [SerializeField] private CombatSettings _combatSettings;
        public CombatSettings Combat => _combatSettings;
        
        public string CharacterName => _characterName;
        public float  CharacterMass => _characterMass;
    }
    
    [System.Serializable]
    public struct MovementSettings
    {
        [Header("Ground Movement")]
        [Range(0f,  20f)] public float RunSpeed;
        [Range(0f,  10f)] public float WalkSpeed;
        [Range(0f, 100f)] public float Acceleration;
        [Range(0f, 100f)] public float Deceleration;
        
        [Header("Jump")]
        [Range(0f, 20f)] public float JumpImpulse;
        [Range(0f,  5f)] public float AirborneGravityMultiplier;
        
        [Header("Air Control")]
        [Range(0f,  1f)] public float AirControlFactor;
        [Range(0f,  5f)] public float FallingDelay;
    }

    [System.Serializable]
    public struct CombatSettings
    {
        [Header("Simple Attack")]
        [Range(0f,  50f)] public float SimpleAttackKnockback;
        [Range(0.1f, 2f)] public float SimpleAttackDuration;
        [Range(0f,   2f)] public float SimpleAttackHitboxStart;
        [Range(0f,   2f)] public float SimpleAttackHitboxEnd;

        [Header("Powered Attack")]
        [Range(0f,  50f)] public float PoweredAttackKnockback;
        [Range(0.1f, 5f)] public float PoweredAttackDuration;
    }
}
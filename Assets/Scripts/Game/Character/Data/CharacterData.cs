using UnityEngine;

namespace Game.Character
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Characters/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private string _characterName = "str_ortiz";
        [SerializeField] private float  _mass = 80f;
        
        [Header("Movement")]
        [SerializeField] private MovementSettings _movement = new MovementSettings();
        
        public string CharacterName => _characterName;
        public float Mass           => _mass;

        public MovementSettings Movement => _movement;
    }
    
    [System.Serializable]
    public struct MovementSettings
    {
        [Header("Speeds")]
        public float RunSpeed;
        public float WalkSpeed;
        public float Acceleration;
        public float Deceleration;
        
        [Header("Jump")]
        public float JumpImpulse;
        public float AirborneGravityMultiplier;
        
        [Header("Air Control")]
        [Range(0f, 1f)]
        public float AirControlFactor;
        public float MaxAirSpeed;
        public float AirBrakingForce;
        
        public MovementSettings(float defaultValue)
        {
            RunSpeed                  = 6f;
            WalkSpeed                 = 3f;
            Acceleration              = 12f;
            Deceleration              = 10f;
            JumpImpulse               = 8f;
            AirborneGravityMultiplier = 2f;
            AirControlFactor          = 0.3f;
            MaxAirSpeed               = 4f;
            AirBrakingForce           = 8f;
        }
    }
}
using UnityEngine;

namespace Game.Core.Character
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
        [Header("Ground Movement")]
        [Range(0f, 20f)]  public float RunSpeed;
        [Range(0f, 10f)]  public float WalkSpeed;
        [Range(0f, 100f)] public float Acceleration;
        [Range(0f, 100f)] public float Deceleration;
        
        [Header("Jump")]
        [Range(0f, 20f)]  public float JumpImpulse;
        [Range(0f, 5f)]   public float AirborneGravityMultiplier;
        
        [Header("Air Control")]
        [Range(0f, 1f)]   public float AirControlFactor;
    }
}
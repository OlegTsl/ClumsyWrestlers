using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Character
{
    public class CharacterView : MonoBehaviour, ICharacterView
    {
        [SerializeField] private CharacterData   _data;
        [SerializeField] private Rigidbody       _rigidbody;
        [SerializeField] private Animator        _animator;
        [SerializeField] private Collider        _hitbox;
        [SerializeField] private Camera          _camera;
        [SerializeField] private Collider        _leftArmCollider;
        [SerializeField] private Collider        _rightArmCollider;
        [SerializeField] private Collider        _leftLegCollider;
        [SerializeField] private Collider        _rightLegCollider;

        private IReadOnlyList<Collider> _attackColliders;
        
        public CharacterData           Data             => _data;
        public Collider                Hitbox           => _hitbox;
        public Collider                LeftArmCollider  => _leftArmCollider;
        public Collider                RightArmCollider => _rightArmCollider;
        public Collider                LeftLegCollider  => _leftLegCollider;
        public Collider                RightLegCollider => _rightLegCollider;
        public IReadOnlyList<Collider> AttackColliders  => _attackColliders;
        public Transform               Transform        => transform;
        public Transform               CameraTransform  => _camera.transform;

        public event Action<Collider> OnHitTrigger;

        private void Awake()
        {
            _attackColliders = new Collider[]
            {
                LeftArmCollider,
                RightArmCollider,
                LeftLegCollider,
                RightLegCollider
            };
        }

        public void Hide()
            => gameObject.SetActive(false);

        public void Show()
            => gameObject.SetActive(true);

        public Action DisposeAction { get; set; }

        public void SetCameraEnabled(bool isPlayer)
            => _camera.enabled = isPlayer;

        public void SetPosition(Vector3 position)
            => transform.position = position;

        public void SetRotation(Quaternion rotation)
            => transform.rotation = rotation;

        public void SetAnimatorFloat(int id, float value, float dampTime, float deltaTime)
            => _animator.SetFloat(id, value, dampTime, deltaTime);

        public void SetAnimatorBool(int id, bool value)
            => _animator.SetBool(id, value);

        public void SetAnimatorTrigger(int id)
            => _animator.SetTrigger(id);

        public void SetVelocity(Vector3 velocity)
            => _rigidbody.velocity = velocity;

        public bool IsGrounded()
            => Physics.Raycast(_rigidbody.position + Vector3.up * 0.1f, Vector3.down, 0.2f);

        public bool IsMoving()
            => _rigidbody.velocity.magnitude > 0.1f;

        public Vector3 TransformDirection(Vector3 direction)
            => transform.TransformDirection(direction);

        public Vector3 InverseTransformDirection(Vector3 direction)
            => transform.InverseTransformDirection(direction);

        public Vector3 GetVelocity()
            => _rigidbody.velocity;

        private void OnTriggerEnter(Collider other)
            => OnHitTrigger?.Invoke(other);
    }
}
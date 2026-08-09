using System;
using System.Collections.Generic;
using Game.Core.Data;
using UnityEngine;

namespace Game.Core.Character
{
    public class CharacterView : MonoBehaviour, ICharacterView
    {
        [SerializeField] private CharacterData   _data;
        [SerializeField] private Rigidbody       _rigidbody;
        [SerializeField] private Animator        _animator;
        [SerializeField] private Collider        _hitbox;
        [SerializeField] private Collider        _leftArmCollider;
        [SerializeField] private Collider        _rightArmCollider;
        [SerializeField] private Collider        _leftLegCollider;
        [SerializeField] private Collider        _rightLegCollider;
        [SerializeField] private LineRenderer    _aim;

        private IReadOnlyList<Collider> _attackColliders;
        private bool _isGrounded = true;
        
        public CharacterData           Data             => _data;
        public Collider                Hitbox           => _hitbox;
        public Collider                LeftArmCollider  => _leftArmCollider;
        public Collider                RightArmCollider => _rightArmCollider;
        public Collider                LeftLegCollider  => _leftLegCollider;
        public Collider                RightLegCollider => _rightLegCollider;
        public IReadOnlyList<Collider> AttackColliders  => _attackColliders;
        public Transform               Transform        => transform;
        public LineRenderer            Aim              => _aim;

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

        public void SetPosition(Vector3 position)
            => transform.position = position;

        public Vector3 GetPosition()
            => transform.position;

        public void SetRotation(Quaternion rotation)
            => transform.rotation = rotation;

        public void SetAnimatorFloat(int id, float value, float dampTime, float deltaTime)
            => _animator.SetFloat(id, value, dampTime, deltaTime);

        public void SetAnimatorBool(int id, bool value)
            => _animator.SetBool(id, value);

        public void SetAnimatorTrigger(int id)
            => _animator.SetTrigger(id);

        public void SetAimPositions(Vector3[] positions)
            => _aim.SetPositions(positions);

        public void SetAimPositionCount(int count)
            => _aim.positionCount = count;

        public void SetVelocity(Vector3 velocity)
            => _rigidbody.velocity = velocity;

        public bool IsGrounded()
            => _isGrounded;

        public bool IsMoving()
            => _rigidbody.velocity.magnitude > 0.1f;

        public Vector3 TransformDirection(Vector3 direction)
            => transform.TransformDirection(direction);

        public Vector3 InverseTransformDirection(Vector3 direction)
            => transform.InverseTransformDirection(direction);

        public Vector3 GetVelocity()
            => _rigidbody.velocity;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerData.Hitbox)
                OnHitTrigger?.Invoke(other);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.layer == LayerData.Ground)
                _isGrounded = false;
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.layer == LayerData.Ground)
                _isGrounded = true;
        }
    }
}
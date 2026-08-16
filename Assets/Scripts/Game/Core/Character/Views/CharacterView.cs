using System;
using Game.Core.Data;
using UnityEngine;

namespace Game.Core.Character
{
    public class CharacterView : MonoBehaviour, ICharacterView
    {
        private const int UpperBodyAttackLayerIndex = 1;

        [SerializeField] private CharacterData   _data;
        [SerializeField] private Rigidbody       _rigidbody;
        [SerializeField] private Animator        _animator;
        [SerializeField] private Collider        _hitbox;
        [SerializeField] private Transform       _attackOrigin;
        [SerializeField] private Transform       _visualRoot;
        [SerializeField] private LineRenderer    _aim;

        private bool       _isGrounded = true;
        private float      _attackHandIkWeight;

        private Vector3    _attackHandIkPosition;
        private Transform  _leftHand;
        private Transform  _rightHand;
        private AttackHand _attackHand;
        private Quaternion _visualRootBaseRotation;
        
        public CharacterData           Data             => _data;
        public Collider                Hitbox           => _hitbox;
        public Transform               Transform        => transform;
        public Transform               AttackOrigin     => _attackOrigin;
        public LineRenderer            Aim              => _aim;

        private void Awake()
        {
            _leftHand               = _animator.GetBoneTransform(HumanBodyBones.LeftHand);
            _rightHand              = _animator.GetBoneTransform(HumanBodyBones.RightHand);
            _visualRootBaseRotation = _visualRoot.localRotation;
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

        public void SetVisualLean(Quaternion rotation)
            => _visualRoot.localRotation = _visualRootBaseRotation * rotation;

        public void SetAnimatorFloat(int id, float value, float dampTime, float deltaTime)
            => _animator.SetFloat(id, value, dampTime, deltaTime);

        public void SetAnimatorBool(int id, bool value)
            => _animator.SetBool(id, value);

        public void SetAnimatorTrigger(int id)
            => _animator.SetTrigger(id);

        public void SetAttackHandIk(AttackHand hand, Vector3 position, float weight)
        {
            _attackHand           = hand;
            _attackHandIkPosition = position;
            _attackHandIkWeight   = Mathf.Clamp01(weight);
        }

        public void ClearAttackHandIk()
            => _attackHandIkWeight = 0f;

        private void OnDisable()
        {
            ClearAttackHandIk();
            SetVisualLean(Quaternion.identity);
        }

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

        private void OnAnimatorIK(int layerIndex)
        {
            if (layerIndex != UpperBodyAttackLayerIndex)
                return;

            AvatarIKGoal activeGoal = _attackHand == AttackHand.Left
                ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;

            AvatarIKGoal inactiveGoal = _attackHand == AttackHand.Left
                ? AvatarIKGoal.RightHand : AvatarIKGoal.LeftHand;

            _animator.SetIKPositionWeight(inactiveGoal, 0f);
            _animator.SetIKPositionWeight(activeGoal, _attackHandIkWeight);

            if (_attackHandIkWeight > 0f)
            {
                Transform activeHand = _attackHand == AttackHand.Left
                    ? _leftHand : _rightHand;

                Vector3 targetPosition = _attackHandIkPosition;

                if (activeHand != null)
                    targetPosition.y = activeHand.position.y;

                _animator.SetIKPosition(activeGoal, targetPosition);
            }
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

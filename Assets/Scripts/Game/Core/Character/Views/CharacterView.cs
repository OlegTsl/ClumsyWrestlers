using System.Collections.Generic;
using Game.Core.Data;
using UnityEngine;

namespace Game.Core.Character
{
    public sealed class CharacterView : MonoBehaviour, ICharacterView
    {
        private const int UpperBodyAttackLayerIndex = 1;

        [SerializeField] private CharacterData _data;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Animator _animator;
        [SerializeField] private Collider _hitbox;
        [SerializeField] private Transform _attackOrigin;
        [SerializeField] private Transform _visualRoot;
        [SerializeField] private LineRenderer _aim;

        private readonly HashSet<Collider> _groundColliders = new(4);

        private float _attackHandIkWeight;
        private Vector3 _attackHandIkPosition;
        private Transform _leftHand;
        private Transform _rightHand;
        private AttackHand _attackHand;
        private Quaternion _visualRootBaseRotation;

        public CharacterData Data => _data;
        public Collider Hitbox => _hitbox;
        public Rigidbody Rigidbody => _rigidbody;
        public Transform Transform => transform;

        private void Awake()
        {
            _leftHand = _animator.GetBoneTransform(HumanBodyBones.LeftHand);
            _rightHand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
            _visualRootBaseRotation = _visualRoot.localRotation;
        }

        private void OnDisable()
        {
            _groundColliders.Clear();
            ClearAttackHandIk();
            SetVisualLean(Quaternion.identity);
        }

        public void Hide()
            => gameObject.SetActive(false);

        public void Show()
            => gameObject.SetActive(true);

        public CharacterPhysicsSnapshot CapturePhysicsSnapshot()
            => new(
                _rigidbody.position,
                _rigidbody.rotation,
                _rigidbody.velocity,
                _attackOrigin.position,
                _groundColliders.Count > 0);

        public void SetPosition(Vector3 position)
            => _rigidbody.position = position;

        public void SetRotation(Quaternion rotation)
            => _rigidbody.rotation = rotation;

        public void MoveRotation(Quaternion rotation)
            => _rigidbody.MoveRotation(rotation);

        public void SetVelocity(Vector3 velocity)
            => _rigidbody.velocity = velocity;

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
            _attackHand = hand;
            _attackHandIkPosition = position;
            _attackHandIkWeight = Mathf.Clamp01(weight);
        }

        public void ClearAttackHandIk()
            => _attackHandIkWeight = 0f;

        public void SetAimEnabled(bool enabled)
            => _aim.gameObject.SetActive(enabled);

        public void SetAimPositions(Vector3[] positions, int count)
        {
            _aim.positionCount = count;
            for (int i = 0; i < count; i++)
            {
                _aim.SetPosition(i, positions[i]);
            }
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (layerIndex != UpperBodyAttackLayerIndex)
            {
                return;
            }

            AvatarIKGoal activeGoal = _attackHand == AttackHand.Left
                ? AvatarIKGoal.LeftHand
                : AvatarIKGoal.RightHand;
            AvatarIKGoal inactiveGoal = _attackHand == AttackHand.Left
                ? AvatarIKGoal.RightHand
                : AvatarIKGoal.LeftHand;

            _animator.SetIKPositionWeight(inactiveGoal, 0f);
            _animator.SetIKPositionWeight(activeGoal, _attackHandIkWeight);

            if (_attackHandIkWeight <= 0f)
            {
                return;
            }

            Transform activeHand = _attackHand == AttackHand.Left ? _leftHand : _rightHand;
            Vector3 targetPosition = _attackHandIkPosition;
            if (activeHand != null)
            {
                targetPosition.y = activeHand.position.y;
            }

            _animator.SetIKPosition(activeGoal, targetPosition);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerData.Ground)
            {
                _groundColliders.Add(collision.collider);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.layer == LayerData.Ground)
            {
                _groundColliders.Remove(collision.collider);
            }
        }
    }
}

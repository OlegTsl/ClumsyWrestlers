using System;
using UnityEngine;

namespace Game.Core.Environment
{
    public sealed class InteractableChestView : MonoBehaviour, IInteractableChestView
    {
        private const float CMinimumArmedSpeedSqr = 0.09f;

        [SerializeField] private EnvironmentData _data;
        [SerializeField] private Rigidbody       _rigidbody;
        [SerializeField] private Collider        _hitbox;

        private EnvironmentCollisionData _pendingCollision;
        private Guid                     _chestID;
        private Vector3                  _lastHorizontalVelocity;

        private float _armedAtFixedTime;
        private float _predictedAtFixedTime;
        private bool  _canDamageTargets;
        private bool  _hasPredictedVelocity;

        public Guid ChestID         => _chestID;
        public EnvironmentData Data => _data;
        public Collider Hitbox      => _hitbox;
        public Vector3 Position     => _rigidbody.position;

        private void Awake()
        {
            _chestID        = Guid.NewGuid();
            _rigidbody.drag = _data.ForceDamping;
        }

        private void FixedUpdate()
        {
            if (!_canDamageTargets || Time.fixedTime <= _armedAtFixedTime + Time.fixedDeltaTime)
                return;

            Vector3 velocity = _rigidbody.velocity;
            velocity.y       = 0f;

            if (!_hasPredictedVelocity || Time.fixedTime > _predictedAtFixedTime)
            {
                _lastHorizontalVelocity = velocity;
                _hasPredictedVelocity   = false;
            }

            if (velocity.sqrMagnitude < CMinimumArmedSpeedSqr)
                _canDamageTargets = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_canDamageTargets)
                return;

            Vector3 velocity = _rigidbody.velocity;
            velocity.y       = 0f;

            Vector3 impactVelocity = _lastHorizontalVelocity.sqrMagnitude > velocity.sqrMagnitude
                ? _lastHorizontalVelocity : velocity;

            if (impactVelocity.sqrMagnitude < CMinimumArmedSpeedSqr)
                return;

            _pendingCollision = new EnvironmentCollisionData(collision.collider, impactVelocity);
        }

        public void ApplyImpulse(Vector3 impulse)
        {
            if (impulse.sqrMagnitude <= Mathf.Epsilon)
                return;

            _canDamageTargets     = true;
            _armedAtFixedTime     = Time.fixedTime;
            _predictedAtFixedTime = Time.fixedTime;

            Vector3 predictedVelocity = _rigidbody.velocity + impulse / _rigidbody.mass;
            predictedVelocity.y       = 0f;

            _lastHorizontalVelocity = predictedVelocity;
            _hasPredictedVelocity   = true;

            _rigidbody.AddForce(impulse, ForceMode.Impulse);
        }

        public void DampenAfterImpact(Vector3 impactVelocity)
        {
            float retention = 1f / (1f + _data.ForceDamping);

            Vector3 velocity = _rigidbody.velocity;
            velocity.x       = impactVelocity.x * retention;
            velocity.z       = impactVelocity.z * retention;

            _rigidbody.velocity = velocity;

            _lastHorizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
            _hasPredictedVelocity   = false;
        }

        public bool TryConsumeCollision(out EnvironmentCollisionData collision)
        {
            if (_pendingCollision == null)
            {
                collision = default;
                return false;
            }

            collision = _pendingCollision;
            _pendingCollision = null;

            return true;
        }
    }
}

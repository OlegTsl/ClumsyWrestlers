using Game.Core.Level.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Environment
{
    public sealed class InteractableChestView :
        LevelEntityView,
        ILevelEntityImpactSettingsProvider,
        IImpulseReceiverView,
        IImpactSourceView,
        ILevelCollisionEmitter,
        ILevelEntityFixedTickView
    {
        private const float MinimumArmedSpeedSqr = 0.09f;

        [SerializeField] private EnvironmentData _data;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _hitbox;

        private ILevelCollisionSink _collisionSink;
        private Vector3 _lastHorizontalVelocity;
        private float _armedAtFixedTime;
        private float _predictedAtFixedTime;
        private bool _canDamageTargets;
        private bool _hasPredictedVelocity;
        private readonly Collider[] _registeredHitboxes = new Collider[1];

        public override IReadOnlyList<Collider> Hitboxes => _registeredHitboxes;
        public override Vector3 Position => _rigidbody.position;
        public LevelEntityImpactSettings ImpactSettings => new(
            _data.HitForceMultiplier,
            _data.TargetImpactForce);

        private void Awake()
        {
            _registeredHitboxes[0] = _hitbox;
            _rigidbody.drag = _data.ForceDamping;
        }

        public void FixedTick()
        {
            if (!_canDamageTargets || Time.fixedTime <= _armedAtFixedTime + Time.fixedDeltaTime)
            {
                return;
            }

            Vector3 velocity = _rigidbody.velocity;
            velocity.y = 0f;

            if (!_hasPredictedVelocity || Time.fixedTime > _predictedAtFixedTime)
            {
                _lastHorizontalVelocity = velocity;
                _hasPredictedVelocity = false;
            }

            if (velocity.sqrMagnitude < MinimumArmedSpeedSqr)
            {
                _canDamageTargets = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_canDamageTargets || _collisionSink == null)
            {
                return;
            }

            Vector3 velocity = _rigidbody.velocity;
            velocity.y = 0f;

            Vector3 impactVelocity = _lastHorizontalVelocity.sqrMagnitude > velocity.sqrMagnitude
                ? _lastHorizontalVelocity
                : velocity;
            if (impactVelocity.sqrMagnitude < MinimumArmedSpeedSqr)
            {
                return;
            }

            LevelCollisionEvent collisionEvent = new(
                EntityId,
                collision.collider,
                impactVelocity);
            _collisionSink.TryEnqueue(collisionEvent);
        }

        public void BindCollisionSink(ILevelCollisionSink collisionSink)
            => _collisionSink = collisionSink;

        public void ApplyImpulse(Vector3 impulse)
        {
            if (impulse.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            _canDamageTargets = true;
            _armedAtFixedTime = Time.fixedTime;
            _predictedAtFixedTime = Time.fixedTime;

            Vector3 predictedVelocity = _rigidbody.velocity + impulse / _rigidbody.mass;
            predictedVelocity.y = 0f;

            _lastHorizontalVelocity = predictedVelocity;
            _hasPredictedVelocity = true;
            _rigidbody.AddForce(impulse, ForceMode.Impulse);
        }

        public void DampenAfterImpact(Vector3 impactVelocity)
        {
            float retention = 1f / (1f + _data.ForceDamping);
            Vector3 velocity = _rigidbody.velocity;
            velocity.x = impactVelocity.x * retention;
            velocity.z = impactVelocity.z * retention;
            _rigidbody.velocity = velocity;

            _lastHorizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
            _hasPredictedVelocity = false;
        }
    }
}

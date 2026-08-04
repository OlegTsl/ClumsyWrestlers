using System;
using UnityEngine;

namespace Game.Core.Character
{
    public sealed class CharacterModel : ICharacterModel
    { 
        private ICharacterView _view;
        private Guid           _id;

        private bool _enabled;
        private bool _hitsEnabled;

        public Guid          CharacterID     => _id;
        public CharacterData Data            => _view.Data;
        public Collider      Hitbox          => _view.Hitbox;
        public Transform     Transform       => _view.Transform;
        public Transform     CameraTransform => _view.CameraTransform;
        public bool          Enabled         => _enabled;

        public event Action<Collider> OnHitTrigger;
 
        public CharacterModel(
            ICharacterView view,
            Guid           characterId
        )
        {
            _view = view;
            _id   = characterId;

            _view.OnHitTrigger += OnHitTriggered;
        }

        public void SetPosition(Vector3 position)
            => _view.SetPosition(position);

        public void SetRotation(Quaternion rotation)
            => _view.SetRotation(rotation);

        public void SetEnabled(bool enabled)
        {
            _enabled = enabled;

            if (enabled)
                _view.Show();
            else
                _view.Hide();
        }

        public void SetAnimatorFloat(int id, float value, float dampTime, float deltaTime)
            => _view.SetAnimatorFloat(id, value, dampTime, deltaTime);

        public void SetAnimatorBool(int id, bool value)
            => _view.SetAnimatorBool(id, value);

        public void SetAnimatorTrigger(int id)
            => _view.SetAnimatorTrigger(id);

        public bool IsGrounded()
            => _view.IsGrounded();

        public bool IsMoving()
            => _view.IsMoving();

        public void ApplyVelocity(Vector3 velocity)
            => _view.SetVelocity(velocity);

        public Vector3 TransformDirection(Vector3 direction)
            => _view.TransformDirection(direction);

        public Vector3 InverseTransformDirection(Vector3 direction)
            => _view.InverseTransformDirection(direction);

        public Vector3 GetVelocity()
            => _view.GetVelocity();

        public void SetHitsEnabled(bool enabled)
            => _hitsEnabled = enabled;

        public void SetCameraEnabled(bool enabled)
            => _view.SetCameraEnabled(enabled);

        private void OnHitTriggered(Collider other)
        {
            if (_hitsEnabled)
                OnHitTrigger?.Invoke(other);
        }

        public void Dispose()
        {
            _view.OnHitTrigger -= OnHitTriggered;
        }
    }
}
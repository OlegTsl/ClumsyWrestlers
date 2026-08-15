using System;
using UnityEngine;

namespace Game.Core.Character
{
    public sealed class CharacterModel : ICharacterModel
    { 
        private ICharacterView _view;
        private Guid           _id;

        private bool _enabled;
        private bool _isMovable = true;

        public Guid          CharacterID     => _id;
        public CharacterData Data            => _view.Data;
        public Collider      Hitbox          => _view.Hitbox;
        public Transform     Transform       => _view.Transform;
        public LineRenderer  Aim             => _view.Aim;
        public Vector3       Forward         => _view.Transform.forward;
        public Vector3       AttackOrigin    => _view.AttackOrigin.position;
        public Vector3       Position        => _view.Transform.position;
        public Quaternion    Rotation        => _view.Transform.rotation;
        public bool          Enabled         => _enabled;
        public bool          IsMovable       => _isMovable;
 
        public CharacterModel(
            ICharacterView view,
            Guid           characterId
        )
        {
            _view = view;
            _id   = characterId;
        }

        public void SetPosition(Vector3 position)
            => _view.SetPosition(position);

        public Vector3 GetPosition()
            => _view.GetPosition();

        public void SetRotation(Quaternion rotation)
            => _view.SetRotation(rotation);

        public void SetVisualLean(Quaternion localRotation)
            => _view.SetVisualLean(localRotation);

        public void SetEnabled(bool enabled)
        {
            _enabled = enabled;

            if (enabled)
                _view.Show();
            else
                _view.Hide();
        }

        public void SetMovable(bool isMovable)
            => _isMovable = isMovable;

        public void SetAimEnabled(bool enabled)
            => _view.Aim.gameObject.SetActive(enabled);

        public void SetAnimatorFloat(int id, float value, float dampTime, float deltaTime)
            => _view.SetAnimatorFloat(id, value, dampTime, deltaTime);

        public void SetAnimatorBool(int id, bool value)
            => _view.SetAnimatorBool(id, value);

        public void SetAnimatorTrigger(int id)
            => _view.SetAnimatorTrigger(id);

        public void SetAttackHandIk(
            AttackHand hand,
            Vector3 position,
            float weight
        )
            => _view.SetAttackHandIk(hand, position, weight);

        public void ClearAttackHandIk()
            => _view.ClearAttackHandIk();

        public void SetAimPositions(Vector3[] positions)
            => _view.SetAimPositions(positions);

        public void SetAimPositionCount(int count)
            => _view.SetAimPositionCount(count);

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

        public void Dispose()
        {
            if (_view == null)
                return;

            _enabled = false;
            _view.DisposeAction?.Invoke();
            _view = null;
        }
    }
}

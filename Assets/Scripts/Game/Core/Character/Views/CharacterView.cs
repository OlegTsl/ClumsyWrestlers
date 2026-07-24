using System;
using System.Collections.Generic;
using Game.Core.Components;
using UnityEngine;

namespace Game.Core.Character
{
    public class CharacterView : MonoBehaviour, ICharacterView
    {
        [SerializeField] private CharacterData   _data;
        [SerializeField] private Rigidbody       _rigidbody;
        [SerializeField] private Animator        _animator;
        [SerializeField] private CapsuleCollider _bodyCollider;
        [SerializeField] private Camera          _characterCamera;
        [SerializeField] private Collider        _leftArmCollider;
        [SerializeField] private Collider        _rightArmCollider;
        [SerializeField] private Collider        _leftLegCollider;
        [SerializeField] private Collider        _rightLegCollider;
        [SerializeField] private ColliderHandler _colliderHandler;

        private IReadOnlyList<Collider> _attackColliders;
        
        public CharacterData           Data             => _data;
        public Rigidbody               Rigidbody        => _rigidbody;
        public Animator                Animator         => _animator;
        public Collider                HitBox           => _bodyCollider;
        public Collider                LeftArmCollider  => _leftArmCollider;
        public Collider                RightArmCollider => _rightArmCollider;
        public Collider                LeftLegCollider  => _leftLegCollider;
        public Collider                RightLegCollider => _rightLegCollider;
        public Transform               Transform        => transform;
        public Camera                  CharacterCamera  => _characterCamera;
        public ColliderHandler         ColliderHandler  => _colliderHandler;
        public IReadOnlyList<Collider> AttackColliders  => _attackColliders;

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

        public void SetAsPlayer(bool isPlayer)
            => _characterCamera.enabled = isPlayer;
    }
}
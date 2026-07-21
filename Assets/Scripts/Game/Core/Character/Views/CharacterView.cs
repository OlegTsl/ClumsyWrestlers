using System;
using UnityEngine;

namespace Game.Core.Character
{
    public class CharacterView : MonoBehaviour, ICharacterView
    {
        [SerializeField] private CharacterData   _data;
        [SerializeField] private Rigidbody       _rigidbody;
        [SerializeField] private Animator        _animator;
        [SerializeField] private CapsuleCollider _bodyCollider;
        [SerializeField] private Collider[]      _attackColliders;
        [SerializeField] private Camera          _characterCamera;
        
        public CharacterData Data           => _data;
        public Rigidbody Rigidbody          => _rigidbody;
        public Animator Animator            => _animator;
        public CapsuleCollider BodyCollider => _bodyCollider;
        public Collider[] AttackColliders   => _attackColliders;
        public Transform Transform          => transform;
        public Camera    CharacterCamera    => _characterCamera;

        public void Hide() => gameObject.SetActive(false);
        public void Show() => gameObject.SetActive(true);

        public Action DisposeAction { get; set; }
    }
}
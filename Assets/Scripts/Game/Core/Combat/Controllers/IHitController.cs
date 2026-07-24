using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Combat
{
    public interface IHitController
    {
        void Initialize(IReadOnlyList<Collider> colliders);
        void OnTriggerEnter(Collider collider);
        void Enable();
        void Disable();
        event Action<Collider> OnHit;
    }
}

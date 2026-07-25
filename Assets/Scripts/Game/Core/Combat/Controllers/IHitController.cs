using System;
using UnityEngine;

namespace Game.Core.Combat
{
    public interface IHitController
    {
        void OnTriggerEnter(Collider collider);
        void Enable();
        void Disable();
        event Action<Collider> OnHit;
    }
}

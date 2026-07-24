using Game.Core.Combat;
using UnityEngine;

namespace Game.Core.Components
{
    public class ColliderHandler : MonoBehaviour
    {
        private IHitController _hitController;

        public void Initialize(IHitController hitController)
        {
            _hitController = hitController;
        }

        private void OnTriggerEnter(Collider other)
            => _hitController.OnTriggerEnter(other);
    }
}
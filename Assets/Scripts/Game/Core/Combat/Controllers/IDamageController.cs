using Game.Core.Character;
using Game.Core.Movement;
using UnityEngine;

namespace Game.Core.Combat
{
    public interface IDamageController
    {
        void Initialize(
            Transform           transform,
            AttackSettings      settings,
            IHitController      hitController,
            IMovementController movementController);
    }
}

using Game.Core.Animation;
using Game.Core.Character;

namespace Game.Core.Combat
{
    public interface ICombatController
    {
        void Initialize(
            ICharacterView       view,
            IAnimationController animationController,
            IHitController       hitController);

        void FixedTick();
        void Enable();
        void Disable();
    }
}
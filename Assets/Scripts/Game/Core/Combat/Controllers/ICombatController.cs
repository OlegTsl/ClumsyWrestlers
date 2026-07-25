namespace Game.Core.Combat
{
    public interface ICombatController
    {
        void FixedTick();
        void Enable();
        void Disable();
    }
}
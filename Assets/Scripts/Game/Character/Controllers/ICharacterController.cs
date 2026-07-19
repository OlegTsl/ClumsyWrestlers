namespace Game.Character
{
    public interface ICharacterController
    {
        void Initialize(ICharacterView view);
        void FixedTick();
        void Enable();
        void Disable();
    }
}
namespace Game.Core.Character
{
    public interface ICharacterController
    {
        void Initialize(ICharacterView view);
        void Enable();
        void Disable();
    }
}
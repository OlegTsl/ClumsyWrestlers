namespace Game.Core.Character
{
    public interface ICharacterController
    {
        void Initialize(ICharacterContext context);
        void Enable();
        void Disable();
    }
}
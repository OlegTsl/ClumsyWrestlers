namespace Game.Core.Character
{
    public interface IEnemyController
    {
        void Initialize(ICharacterView view);
        void Enable();
        void Disable();
    }
}
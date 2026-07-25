namespace Game.Core.Character
{
    public interface IEnemyController
    {
        void Initialize(ICharacterContext context);
        void Enable();
        void Disable();
    }
}
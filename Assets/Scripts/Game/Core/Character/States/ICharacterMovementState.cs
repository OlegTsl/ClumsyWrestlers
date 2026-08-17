namespace Game.Core.Character
{
    public interface ICharacterMovementState : ICharacterModel
    {
        bool IsMovable { get; }
        void SetMovable(bool isMovable);
    }
}

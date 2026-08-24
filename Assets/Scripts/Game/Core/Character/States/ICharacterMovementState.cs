namespace Game.Core.Character
{
    [System.Flags]
    public enum CharacterControlLock
    {
        None        = 0,
        PowerAttack = 1 << 0,
        Knockdown   = 1 << 1,
        HitReaction = 1 << 2
    }

    public interface ICharacterMovementState : ICharacterModel
    {
        bool IsMovable { get; }
        void SetControlLock(CharacterControlLock controlLock, bool isLocked);
    }
}

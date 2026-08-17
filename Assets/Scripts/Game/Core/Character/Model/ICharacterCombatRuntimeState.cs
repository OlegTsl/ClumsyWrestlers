namespace Game.Core.Character
{
    public interface ICharacterCombatRuntimeState :
        ICharacterTransformState,
        ICharacterActivityState,
        ICharacterMovementState
    {
    }
}

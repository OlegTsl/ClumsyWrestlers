namespace Game.Core.Character
{
    public interface ICharacterCombatRuntimeState :
        ICharacterTransformState,
        ICharacterPhysicsState,
        ICharacterActivityState,
        ICharacterMovementState
    {
    }
}

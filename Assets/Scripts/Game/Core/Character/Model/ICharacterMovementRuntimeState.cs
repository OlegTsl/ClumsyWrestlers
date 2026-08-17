namespace Game.Core.Character
{
    public interface ICharacterMovementRuntimeState :
        ICharacterTransformState,
        ICharacterPhysicsState,
        ICharacterActivityState,
        ICharacterMovementState
    {
    }
}

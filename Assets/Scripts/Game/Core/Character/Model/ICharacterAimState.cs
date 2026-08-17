namespace Game.Core.Character
{
    public interface ICharacterAimState : ICharacterModel
    {
        bool IsAiming { get; }
        void SetAiming(bool isAiming);
    }
}

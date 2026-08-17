namespace Game.Core.Character
{
    public interface ICharacterActivityState : ICharacterModel
    {
        bool Enabled { get; }
        void SetEnabled(bool enabled);
    }
}

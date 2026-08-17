using Game.Core.Entities;

namespace Game.Core.GameEvents
{
    public readonly struct OnAimStateChangedEvent
    {
        public EntityId CharacterID { get; }
        public bool IsAiming { get; }

        public OnAimStateChangedEvent(EntityId characterID, bool isAiming)
        {
            CharacterID = characterID;
            IsAiming = isAiming;
        }
    }
}

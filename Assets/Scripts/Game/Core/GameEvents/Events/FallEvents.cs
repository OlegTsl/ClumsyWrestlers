using Game.Core.Entities;

namespace Game.Core.GameEvents
{
    public readonly struct OnFallEvent
    {
        public EntityId CharacterID { get; }

        public OnFallEvent(EntityId characterID)
            => CharacterID = characterID;
    }
}

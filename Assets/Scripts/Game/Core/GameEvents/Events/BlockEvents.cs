using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.GameEvents
{
    public readonly struct OnBlockEvent
    {
        public EntityId CharacterID { get; }
        public bool     Blocked     { get; }
        public OnBlockEvent(EntityId characterID, bool blocked)
        {
            CharacterID = characterID;
            Blocked     = blocked;
        }
    }
}

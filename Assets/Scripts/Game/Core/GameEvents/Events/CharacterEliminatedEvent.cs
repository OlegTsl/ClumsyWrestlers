using Game.Core.Entities;
using Game.Core.Teams;

namespace Game.Core.GameEvents
{
    public readonly struct OnCharacterEliminatedEvent
    {
        public EntityId CharacterId { get; }
        public TeamId TeamId { get; }

        public OnCharacterEliminatedEvent(
            EntityId characterId,
            TeamId teamId)
        {
            CharacterId = characterId;
            TeamId = teamId;
        }
    }
}

using Game.Core.Character;
using Game.Core.GameEvents;

namespace Game.Core.Systems
{
    public sealed class MovementCommandPublisher
    {
        private readonly IGameEventsBus _events;

        public MovementCommandPublisher(IGameEventsBus events)
            => _events = events;

        public void Publish(ICharacterModel model, CharacterControlState state)
            => _events.Publish(new OnMoveEvent(model.CharacterID, state.MoveDirection));
    }
}

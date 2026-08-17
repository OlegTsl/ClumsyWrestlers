using Game.Core.Character;
using Game.Core.GameEvents;
using UnityEngine;

namespace Game.Core.Systems
{
    public sealed class CharacterMovementCommandPublisher
    {
        private readonly IGameEventsBus _events;

        public CharacterMovementCommandPublisher(IGameEventsBus events)
            => _events = events;

        public void Publish(
            ICharacterModel model,
            CharacterControlState state)
        {
            ICharacterAimState aim = model.GetState<ICharacterAimState>();
            ICharacterTransformState transform =
                model.GetState<ICharacterTransformState>();
            Vector3 direction = aim.IsAiming &&
                                state.MoveDirection != Vector3.zero
                ? transform.Forward
                : state.MoveDirection;
            _events.Publish(new OnMoveEvent(model.CharacterID, direction));
        }
    }
}

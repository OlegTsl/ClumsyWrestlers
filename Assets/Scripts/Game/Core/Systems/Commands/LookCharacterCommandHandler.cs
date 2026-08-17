using Game.Core.Character;
using Game.Core.Commands;
using Game.Core.Data;
using UnityEngine;

namespace Game.Core.Systems
{
    public sealed class LookCharacterCommandHandler : ICharacterCommandHandler
    {
        private readonly ICharacterContext _characters;
        private readonly CharacterControlStateRegistry _states;
        private readonly CharacterMovementCommandPublisher _movementPublisher;

        public CharacterCommandType CommandType => CharacterCommandType.Look;

        public LookCharacterCommandHandler(
            ICharacterContext characters,
            CharacterControlStateRegistry states,
            CharacterMovementCommandPublisher movementPublisher
        )
        {
            _characters = characters;
            _states = states;
            _movementPublisher = movementPublisher;
        }

        public void Handle(in CharacterCommand command)
        {
            ICharacterModel model = _characters.GetModel(command.CharacterId);
            if (model == null ||
                !model.GetState<ICharacterActivityState>().Enabled ||
                !model.GetState<ICharacterAimState>().IsAiming ||
                Mathf.Approximately(command.LookDelta.x, 0f) ||
                !_states.TryGet(command.CharacterId, out CharacterControlState state))
            {
                return;
            }

            ICharacterTransformState transform =
                model.GetState<ICharacterTransformState>();
            float angle = command.LookDelta.x * CommonData.MouseSensitivity;
            transform.SetRotation(
                transform.Rotation * Quaternion.Euler(0f, angle, 0f));
            if (state.MoveDirection != Vector3.zero)
            {
                _movementPublisher.Publish(model, state);
            }
        }
    }
}

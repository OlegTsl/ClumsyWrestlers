using Game.Common.Input;
using Game.Core.Character;
using Game.Core.Commands;
using Game.Core.Extension;
using Game.Core.GameEvents;

namespace Game.Core.Systems
{
    public sealed class PowerAttackCommandHandler : ICommandHandler
    {
        private readonly ICharacterContext    _characters;
        private readonly IGameEventsBus       _events;
        private readonly ControlStateRegistry _states;

        public CharacterCommandType CommandType
            => CharacterCommandType.PowerAttack;

        public PowerAttackCommandHandler(
            ICharacterContext    characters,
            IGameEventsBus       events,
            ControlStateRegistry states
        )
        {
            _characters = characters;
            _events     = events;
            _states     = states;
        }

        public void Handle(in CharacterCommand command)
        {
            ICharacterModel model = _characters.GetModel(command.CharacterId);
            
            if (model == null || !model.GetState<ICharacterActivityState>().Enabled ||
                !_states.TryGet(command.CharacterId, out _))
            {
                return;
            }

            if (command.InputEventType == InputEventType.Released)
                _events.Publish(new OnPowerAttackRequestedEvent(model.CharacterID));
        }
    }
}

using Game.Common.Input;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Commands
{
    public readonly struct CharacterCommand
    {
        public EntityId             CharacterId    { get; }
        public uint                 Tick           { get; }
        public CharacterCommandType Type           { get; }
        public Vector3              Direction      { get; }
        public Vector2              LookDelta      { get; }
        public InputEventType       InputEventType { get; }

        public CharacterCommand(
            EntityId             characterId,
            uint                 tick,
            CharacterCommandType type,
            Vector3              direction      = default,
            Vector2              lookDelta      = default,
            InputEventType       inputEventType = InputEventType.Pressed
        )
        {
            CharacterId    = characterId;
            Tick           = tick;
            Type           = type;
            Direction      = direction;
            LookDelta      = lookDelta;
            InputEventType = inputEventType;
        }
    }
}

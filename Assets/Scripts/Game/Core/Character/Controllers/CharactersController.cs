using System;
using System.Collections.Generic;
using Game.Common.Input;
using Zenject;

namespace Game.Core.Character
{
    public sealed class CharactersController : ICharactersController, ITickable, IFixedTickable, ILateTickable
    {
        private readonly ICharactersRegistry              _registry;
        private readonly Dictionary<Guid, CharacterEntry> _entries = new();

        public CharactersController(
            ICharactersRegistry registry
        )
        {
            _registry = registry;
        }

        public void AddCharacter(ICharacterContext context)
        {
            CharacterEntry entry = null;
            entry = new CharacterEntry(
                context,
                onMove: input  => HandleMove(entry, input),
                onJump: action => HandleJump(entry, action)
            );

            context.InputEvents.Subscribe(entry.OnMove);
            context.InputEvents.Subscribe(entry.OnJump);

            _entries[context.CharacterId] = entry;
            _registry.Register(context);
        }

        public void RemoveCharacter(ICharacterContext context)
        {
            if (!_entries.TryGetValue(context.CharacterId, out var entry))
                return;

            context.InputEvents.Unsubscribe(entry.OnMove);
            context.InputEvents.Unsubscribe(entry.OnJump);

            _entries.Remove(context.CharacterId);
            _registry.Unregister(context);
        }

        public void RemoveAllCharacters()
        {
            foreach (var (_, entry) in _entries)
            {
                RemoveCharacter(entry.Context);
            }
        }

        public void SetCharacterEnabled(Guid characterID, bool enabled)
        {
            if (_entries.TryGetValue(characterID, out var entry))
                entry.IsEnabled = enabled;
        }

        public void Tick()
        {
            foreach (var (_, entry) in _entries)
            {
                if (!entry.IsEnabled)
                    continue;

                entry.Context.Input.Tick();
            }
        }

        public void FixedTick()
        {
            foreach (var (_, entry) in _entries)
            {
                if (!entry.IsEnabled)
                    continue;

                entry.Context.Movement.FixedTick();
                entry.Context.Combat.FixedTick();
            }
        }

        public void LateTick()
        {
            foreach (var (_, entry) in _entries)
            {
                if (!entry.IsEnabled)
                    continue;

                entry.Context.Animation.UpdateMovementState(
                    entry.Context.View.Rigidbody.velocity,
                    entry.HasMoveInput,
                    entry.Context.Movement.IsGrounded
                );

                entry.Context.Look?.LateTick();
            }
        }

        private void HandleMove(CharacterEntry entry, MoveInput input)
        {
            if (!entry.IsEnabled)
                return;

            entry.HasMoveInput = input.Direction.magnitude > 0.01f;
            entry.Context.Movement.SetMoveDirection(input.Direction);
        }

        private void HandleJump(CharacterEntry entry, JumpAction action)
        {
            if (!entry.IsEnabled)
                return;

            if (action.EventType == InputEventType.Pressed && entry.Context.Movement.IsGrounded)
            {
                entry.Context.Movement.Jump();
                entry.Context.Animation.TriggerJump();
            }
        }
    }
}
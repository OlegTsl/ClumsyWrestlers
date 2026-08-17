using Game.Core.Character;
using Game.Core.GameEvents;
using Game.Core.Level;
using Zenject;

namespace Game.Core.Systems
{
    public sealed class ArenaEliminationSystem :
        IArenaEliminationSystem,
        IFixedTickable
    {
        private readonly IGameEventsBus _events;
        private readonly ICharacterContext _characters;
        private readonly ILevelArenaData _arena;

        public ArenaEliminationSystem(
            IGameEventsBus events,
            ICharacterContext characters,
            ILevelArenaData arena)
        {
            _events = events;
            _characters = characters;
            _arena = arena;
        }

        public void FixedTick()
        {
            var characters = _characters.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                ICharacterModel character = characters[i];
                ICharacterActivityState activity =
                    character.GetState<ICharacterActivityState>();
                if (!activity.Enabled ||
                    character.GetState<ICharacterTransformState>().Position.y >=
                    _arena.EliminationHeight)
                {
                    continue;
                }

                activity.SetEnabled(false);
                _events.Publish(new OnCharacterEliminatedEvent(
                    character.CharacterID,
                    character.GetState<ICharacterTeamState>().TeamId));
            }
        }

        public void Dispose()
        {
        }
    }
}

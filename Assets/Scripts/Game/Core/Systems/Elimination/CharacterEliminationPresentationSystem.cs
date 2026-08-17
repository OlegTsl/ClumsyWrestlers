using Game.Core.Character;
using Game.Core.GameEvents;

namespace Game.Core.Systems
{
    public sealed class CharacterEliminationPresentationSystem :
        ICharacterEliminationPresentationSystem
    {
        private readonly IGameEventsBus _events;
        private readonly ICharacterViewContext _views;

        public CharacterEliminationPresentationSystem(
            IGameEventsBus events,
            ICharacterViewContext views)
        {
            _events = events;
            _views = views;
            _events.Subscribe<OnCharacterEliminatedEvent>(OnCharacterEliminated);
        }

        private void OnCharacterEliminated(OnCharacterEliminatedEvent evt)
            => _views.GetView(evt.CharacterId)?.Hide();

        public void Dispose()
            => _events.Unsubscribe<OnCharacterEliminatedEvent>(
                OnCharacterEliminated);
    }
}

using System;
using Game.Core.Character;
using Zenject;

namespace Game.Core.Systems
{
    public sealed class CharacterPhysicsReadSystem : IDisposable, IFixedTickable
    {
        private readonly ICharacterContext _models;
        private readonly ICharacterViewContext _views;

        public CharacterPhysicsReadSystem(
            ICharacterContext models,
            ICharacterViewContext views
        )
        {
            _models = models;
            _views = views;
        }

        public void FixedTick()
        {
            var characters = _models.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                ICharacterModel model = characters[i];
                ICharacterView view = _views.GetView(model.CharacterID);
                if (view != null)
                {
                    CharacterPhysicsSnapshot snapshot = view.CapturePhysicsSnapshot();
                    model.GetState<ICharacterPhysicsState>()
                        .SynchronizePhysics(snapshot);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}

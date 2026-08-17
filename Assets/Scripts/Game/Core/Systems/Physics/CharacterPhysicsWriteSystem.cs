using System;
using Game.Core.Character;
using Game.Core.Extension;
using Zenject;

namespace Game.Core.Systems
{
    public sealed class CharacterPhysicsWriteSystem : IDisposable, IFixedTickable
    {
        private readonly ICharacterContext     _models;
        private readonly ICharacterViewContext _views;

        public CharacterPhysicsWriteSystem(
            ICharacterContext     models,
            ICharacterViewContext views
        )
        {
            _models = models;
            _views  = views;
        }

        public void FixedTick()
        {
            var characters = _models.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                ICharacterModel model = characters[i];
                ICharacterActivityState activity = model.GetState<ICharacterActivityState>();
                
                if (!activity.Enabled)
                    continue;

                ICharacterView view = _views.GetView(model.CharacterID);
                if (view != null)
                {
                    ICharacterTransformState transform = model.GetState<ICharacterTransformState>();
                    ICharacterPhysicsState physics     = model.GetState<ICharacterPhysicsState>();
                    
                    view.MoveRotation(transform.Rotation);
                    view.SetVelocity(physics.Velocity);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}

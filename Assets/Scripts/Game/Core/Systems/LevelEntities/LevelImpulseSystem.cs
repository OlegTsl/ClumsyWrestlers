using System;
using Game.Core.GameEvents;
using Game.Core.Level;
using Game.Core.Level.Entities;

namespace Game.Core.Systems
{
    public sealed class LevelImpulseSystem : IDisposable
    {
        private readonly IGameEventsBus _events;
        private readonly ILevelEntityRegistry _levelEntities;

        public LevelImpulseSystem(
            IGameEventsBus events,
            ILevelEntityRegistry levelEntities
        )
        {
            _events = events;
            _levelEntities = levelEntities;

            _events.Subscribe<OnLevelEntityImpulseRequestedEvent>(
                OnImpulseRequested);
            _events.Subscribe<OnLevelEntityDampingRequestedEvent>(
                OnDampingRequested);
        }

        private void OnImpulseRequested(OnLevelEntityImpulseRequestedEvent evt)
        {
            if (_levelEntities.TryGetCapability(
                    evt.EntityId,
                    out IImpulseReceiverView receiver))
            {
                receiver.ApplyImpulse(evt.Impulse);
            }
        }

        private void OnDampingRequested(OnLevelEntityDampingRequestedEvent evt)
        {
            if (_levelEntities.TryGetCapability(
                    evt.EntityId,
                    out IImpactSourceView source))
            {
                source.DampenAfterImpact(evt.ImpactVelocity);
            }
        }

        public void Dispose()
        {
            _events.Unsubscribe<OnLevelEntityImpulseRequestedEvent>(
                OnImpulseRequested);
            _events.Unsubscribe<OnLevelEntityDampingRequestedEvent>(
                OnDampingRequested);
        }
    }
}

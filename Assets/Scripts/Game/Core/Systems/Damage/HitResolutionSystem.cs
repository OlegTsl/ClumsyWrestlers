using Game.Core.GameEvents;
using UnityEngine;

namespace Game.Core.Systems
{
    public sealed class HitResolutionSystem : IHitResolutionSystem
    {
        private const float MinimumDirectionSqrMagnitude = 0.0001f;

        private readonly IGameEventsBus _events;

        public HitResolutionSystem(IGameEventsBus events)
        {
            _events = events;
            _events.Subscribe<OnHitValidatedEvent>(OnHitValidated);
        }

        private void OnHitValidated(OnHitValidatedEvent evt)
        {
            HitData resolvedHit = evt.Hit.WithForce(CalculateForce(evt.Hit));
            PublishTargetReaction(resolvedHit);
            PublishSourceReaction(resolvedHit);
            _events.Publish(new OnHitResolvedEvent(resolvedHit));
        }

        private Vector3 CalculateForce(in HitData hit)
        {
            Vector3 direction = hit.Direction;
            direction.y = hit.KnockbackHeight;
            return Normalize(direction) * hit.KnockbackForce;
        }

        private void PublishTargetReaction(in HitData hit)
        {
            if (hit.TargetType == HitObjectType.Character)
            {
                if (hit.Force.sqrMagnitude >= MinimumDirectionSqrMagnitude)
                    _events.Publish(new OnForceEvent(hit.TargetID, hit.Force));

                _events.Publish(new OnHitEvent(hit.TargetID));
                return;
            }

            _events.Publish(new OnLevelEntityImpulseRequestedEvent(
                hit.TargetID, hit.Force));
        }

        private void PublishSourceReaction(in HitData hit)
        {
            if (hit.SourceType == HitObjectType.LevelEntity)
            {
                _events.Publish(new OnLevelEntityDampingRequestedEvent(
                    hit.SourceID, hit.ImpactVelocity));
            }
        }

        private static Vector3 Normalize(Vector3 direction)
            => direction.sqrMagnitude < MinimumDirectionSqrMagnitude
                ? Vector3.zero : direction.normalized;

        public void Dispose()
            => _events.Unsubscribe<OnHitValidatedEvent>(OnHitValidated);
    }
}

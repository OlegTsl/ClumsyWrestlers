using Game.Core.Character;
using Game.Core.Extension;
using Game.Core.GameEvents;
using Game.Core.Level;
using Game.Core.Level.Entities;
using UnityEngine;

namespace Game.Core.Systems
{
    public sealed class DamageSystem : IDamageSystem
    {
        private const float MinimumDirectionSqrMagnitude = 0.0001f;

        private readonly IGameEventsBus               _events;
        private readonly ICharacterContext            _characters;
        private readonly ILevelEntityRegistry         _levelEntities;
        private readonly ILevelImpactSettingsRegistry _impactSettings;

        public DamageSystem(
            IGameEventsBus               events,
            ICharacterContext            characters,
            ILevelEntityRegistry         levelEntities,
            ILevelImpactSettingsRegistry impactSettings
        )
        {
            _events         = events;
            _characters     = characters;
            _levelEntities  = levelEntities;
            _impactSettings = impactSettings;
            _events.Subscribe<OnHitDetectedEvent>(OnHitDetected);
        }

        private void OnHitDetected(OnHitDetectedEvent evt)
        {
            HitData hit = evt.Hit;
            if (!CanResolve(hit))
                return;

            HitData resolvedHit = hit.WithForce(CalculateForce(hit));
            ApplyForce(resolvedHit);
            ApplySourceReaction(resolvedHit);
            _events.Publish(new OnHitResolvedEvent(resolvedHit));
        }

        private bool CanResolve(in HitData hit)
        {
            if (hit.SourceType == HitObjectType.Character)
            {
                ICharacterModel source = _characters.GetModel(hit.SourceID);
                if (source == null || !source.GetState<ICharacterActivityState>().Enabled)
                    return false;
            }
            else if (!_impactSettings.TryGetImpactSettings(hit.SourceID, out _))
                return false;

            if (hit.TargetType == HitObjectType.Character)
            {
                ICharacterModel target = _characters.GetModel(hit.TargetID);
                return target != null && target.GetState<ICharacterActivityState>().Enabled;
            }

            return _impactSettings.TryGetImpactSettings(hit.TargetID, out _) &&
                   _levelEntities.TryGetCapability(hit.TargetID, out IImpulseReceiverView _);
        }

        private Vector3 CalculateForce(in HitData hit)
        {
            Vector3 direction = hit.Direction;
            direction.y = hit.KnockbackHeight;
            return Normalize(direction) * hit.KnockbackForce;
        }

        private void ApplyForce(in HitData hit)
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

        private void ApplySourceReaction(in HitData hit)
        {
            if (hit.SourceType == HitObjectType.LevelEntity)
            {
                _events.Publish(new OnLevelEntityDampingRequestedEvent(
                    hit.SourceID, hit.ImpactVelocity));
            }
        }

        private static Vector3 Normalize(Vector3 direction)
            => direction.sqrMagnitude < MinimumDirectionSqrMagnitude ? Vector3.zero : direction.normalized;

        public void Dispose()
            => _events.Unsubscribe<OnHitDetectedEvent>(OnHitDetected);
    }
}

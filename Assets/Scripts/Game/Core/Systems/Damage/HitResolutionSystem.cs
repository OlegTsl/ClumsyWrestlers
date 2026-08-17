using Game.Core.Character;
using Game.Core.GameEvents;
using Game.Core.Level;
using Game.Core.Level.Entities;
using UnityEngine;

namespace Game.Core.Systems
{
    public sealed class HitResolutionSystem : IHitResolutionSystem
    {
        private const float MinimumDirectionSqrMagnitude = 0.0001f;

        private readonly IGameEventsBus _events;
        private readonly ICharacterContext _characters;
        private readonly ILevelImpactSettingsRegistry _impactSettings;

        public HitResolutionSystem(
            IGameEventsBus events,
            ICharacterContext characters,
            ILevelImpactSettingsRegistry impactSettings)
        {
            _events = events;
            _characters = characters;
            _impactSettings = impactSettings;
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
            if (hit.SourceType == HitObjectType.LevelEntity)
            {
                _impactSettings.TryGetImpactSettings(
                    hit.SourceID,
                    out LevelEntityImpactSettings sourceSettings);
                direction.y = 1f;
                return Normalize(direction) * sourceSettings.TargetImpactForce;
            }

            ICharacterModel attacker = _characters.GetModel(hit.SourceID);
            float force = hit.AttackType == AttackType.Power
                ? attacker.Data.Combat.PowerAttack.KnockbackForce
                : attacker.Data.Combat.SimpleAttack.KnockbackForce;
            direction.y = hit.AttackType == AttackType.Power ? 1f : 0f;

            if (hit.TargetType == HitObjectType.LevelEntity)
            {
                _impactSettings.TryGetImpactSettings(
                    hit.TargetID,
                    out LevelEntityImpactSettings targetSettings);
                force *= targetSettings.HitForceMultiplier;
            }

            return Normalize(direction) * force;
        }

        private void PublishTargetReaction(in HitData hit)
        {
            if (hit.TargetType == HitObjectType.Character)
            {
                if (hit.Force.sqrMagnitude >= MinimumDirectionSqrMagnitude)
                {
                    _events.Publish(new OnForceEvent(hit.TargetID, hit.Force));
                }

                _events.Publish(new OnHitEvent(hit.TargetID));
                return;
            }

            _events.Publish(new OnLevelEntityImpulseRequestedEvent(
                hit.TargetID,
                hit.Force));
        }

        private void PublishSourceReaction(in HitData hit)
        {
            if (hit.SourceType == HitObjectType.LevelEntity)
            {
                _events.Publish(new OnLevelEntityDampingRequestedEvent(
                    hit.SourceID,
                    hit.ImpactVelocity));
            }
        }

        private static Vector3 Normalize(Vector3 direction)
            => direction.sqrMagnitude < MinimumDirectionSqrMagnitude
                ? Vector3.zero
                : direction.normalized;

        public void Dispose()
            => _events.Unsubscribe<OnHitValidatedEvent>(OnHitValidated);
    }
}

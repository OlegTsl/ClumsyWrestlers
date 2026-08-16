using Game.Core.Character;
using Game.Core.Environment;
using Game.Core.GameEvents;
using Game.Core.Level;
using UnityEngine;

namespace Game.Core.Systems
{
    public sealed class DamageSystem : IDamageSystem
    {
        private const float CMinimumDirectionSqrMagnitude = 0.0001f;

        private readonly GameEventsBus     _events;
        private readonly ICharacterContext _characterContext;
        private readonly ILevelModel       _level;

        public DamageSystem(
            GameEventsBus     events,
            ICharacterContext characterContext,
            ILevelModel       level
        )
        {
            _events           = events;
            _characterContext = characterContext;
            _level            = level;

            _events.Subscribe<OnHitDetectedEvent>(OnHitDetected);
        }

        private void OnHitDetected(OnHitDetectedEvent evt)
        {
            HitData hit = evt.Hit;
            if (!CanResolve(hit))
            {
                return;
            }

            Vector3 force = CalculateForce(hit);
            HitData resolvedHit = hit.WithForce(force);

            ApplyForce(resolvedHit);
            ApplySourceReaction(resolvedHit);

            _events.Publish(new OnHitResolvedEvent(resolvedHit));
        }

        private bool CanResolve(HitData hit)
        {
            if (hit.SourceType == HitObjectType.Character)
            {
                ICharacterModel source = _characterContext.GetModel(hit.SourceID);
                if (source == null || !source.Enabled)
                {
                    return false;
                }
            }

            if (hit.TargetType == HitObjectType.Character)
            {
                ICharacterModel target = _characterContext.GetModel(hit.TargetID);
                return target != null && target.Enabled;
            }

            return true;
        }

        private Vector3 CalculateForce(HitData hit)
        {
            Vector3 direction = hit.Direction;

            if (hit.SourceType == HitObjectType.Environment)
            {
                direction.y = 1f;
                return Normalize(direction) *
                    _level.GetInteractableChest(hit.SourceID).Data.TargetImpactForce;
            }

            float force = GetCharacterAttackForce(hit);
            if (hit.TargetType == HitObjectType.Environment)
            {
                direction.y = hit.AttackType == AttackType.Power ? 1f : 0f;
                force *= _level.GetInteractableChest(hit.TargetID).Data.HitForceMultiplier;
            }
            else
            {
                direction.y = hit.AttackType == AttackType.Power ? 1f : 0f;
            }

            return Normalize(direction) * force;
        }

        private float GetCharacterAttackForce(HitData hit)
        {
            ICharacterModel attacker = _characterContext.GetModel(hit.SourceID);
            return hit.AttackType == AttackType.Power
                ? attacker.Data.Combat.PowerAttack.KnockbackForce
                : attacker.Data.Combat.SimpleAttack.KnockbackForce;
        }

        private void ApplyForce(HitData hit)
        {
            if (hit.TargetType == HitObjectType.Character)
            {
                if (hit.Force.sqrMagnitude >= CMinimumDirectionSqrMagnitude)
                {
                    _events.Publish(new OnForceEvent(hit.TargetID, hit.Force));
                }

                _events.Publish(new OnHitEvent(hit.TargetID));
                return;
            }

            _level.GetInteractableChest(hit.TargetID).ApplyImpulse(hit.Force);
        }

        private void ApplySourceReaction(HitData hit)
        {
            if (hit.SourceType == HitObjectType.Environment)
            {
                _level.GetInteractableChest(hit.SourceID)
                    .DampenAfterImpact(hit.ImpactVelocity);
            }
        }

        private static Vector3 Normalize(Vector3 direction)
        {
            if (direction.sqrMagnitude < CMinimumDirectionSqrMagnitude)
            {
                return Vector3.zero;
            }

            return direction.normalized;
        }

        public void Dispose()
            => _events.Unsubscribe<OnHitDetectedEvent>(OnHitDetected);
    }
}

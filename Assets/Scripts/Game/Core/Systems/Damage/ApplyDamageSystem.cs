using System;
using Game.Core.Character;
using Game.Core.GameEvents;
using UnityEngine;

namespace Game.Core.Systems
{
    public class ApplyDamageSystem : IApplyDamageSystem, IDisposable
    {
        private readonly GameEventsBus     _gameEventsBus;
        private readonly ICharacterContext _context;
        
        public ApplyDamageSystem(
            GameEventsBus     gameEventsBus,
            ICharacterContext context
        )
        {
            _gameEventsBus = gameEventsBus;
            _context       = context;

            _gameEventsBus.Subscribe<OnHitResolvedEvent>(ApplyDamage);
        }

        private void ApplyDamage(OnHitResolvedEvent evt)
        {
            HitData hit = evt.Hit;
            var attackerModel = _context.GetModel(hit.AttackerID);
            var targetModel = _context.GetModel(hit.TargetID);

            if (attackerModel == null || targetModel == null ||
                !attackerModel.Enabled || !targetModel.Enabled)
            {
                return;
            }

            float force = GetKnockbackForce(hit, attackerModel);
            if (force > 0f)
            {
                ApplyForce(hit, force);
            }

            _gameEventsBus.Publish(new OnHitEvent(hit.TargetID));
        }

        private static float GetKnockbackForce(HitData hit, ICharacterModel attacker)
        {
            if (hit.AttackType == AttackType.Power)
                return attacker.Data.Combat.PowerAttack?.KnockbackForce ?? 0f;

            SimpleAttackSettings settings =
                attacker.Data.Combat.SimpleAttack;
            return settings?.KnockbackForce ?? 0f;
        }

        private void ApplyForce(HitData hit, float force)
        {
            Vector3 direction = hit.Direction;
            direction.y = hit.AttackType == AttackType.Power ? 1f : 0f;
            direction.Normalize();

            _gameEventsBus.Publish(new OnForceEvent(
                hit.TargetID,
                direction * force));
        }

        public void Dispose()
            => _gameEventsBus.Unsubscribe<OnHitResolvedEvent>(ApplyDamage);
    }
}

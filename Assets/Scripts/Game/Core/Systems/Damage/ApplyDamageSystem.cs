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

            _gameEventsBus.Subscribe<OnApplyDamageEvent>(ApplyDamage);
        }

        private void ApplyDamage(OnApplyDamageEvent evt)
        {
            var attackerModel = _context.GetModel(evt.AttackerID);
            var targetModel   = _context.GetModel(evt.TargetID);

            if (attackerModel == null || targetModel == null)
                return;

            Vector3 direction = targetModel.Transform.position -
                attackerModel.Transform.position;

            direction.y = 1.0f;
            direction.Normalize();

            Vector3 force = direction * evt.Force;
            _gameEventsBus.Publish(new OnForceEvent(evt.TargetID, force, true));
        }

        public void Dispose()
            => _gameEventsBus.Unsubscribe<OnApplyDamageEvent>(ApplyDamage);
    }
}
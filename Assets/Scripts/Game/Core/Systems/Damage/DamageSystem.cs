using System;
using Game.Core.Character;
using Game.Core.GameEvents;
using UnityEngine;

namespace Game.Core.Systems
{
    public class DamageSystem : IDamageSystem, IDisposable
    {
        private readonly GameEventsBus     _gameEventsBus;
        private readonly ICharacterContext _context;
        
        public DamageSystem(
            GameEventsBus     gameEventsBus,
            ICharacterContext context
        )
        {
            _gameEventsBus = gameEventsBus;
            _context       = context;

            _gameEventsBus.Subscribe<OnDamageEvent>(ApplyDamage);
        }

        private void ApplyDamage(OnDamageEvent evt)
        {
            var srcModel = _context.GetModel(evt.CharacterID);
            var dstModel = _context.GetModel(evt.TargetID);

            if (srcModel == null || dstModel == null)
                return;

            Vector3 direction = dstModel.Transform.position - srcModel.Transform.position;
            direction.y = 1.0f;
            direction.Normalize();

            var settings = dstModel.Data.Combat;

            Vector3 force = direction * settings.SimpleAttackKnockback;
            _gameEventsBus.Publish(new OnForceEvent(evt.TargetID, force));          
        }

        public void Dispose()
            => _gameEventsBus.Unsubscribe<OnDamageEvent>(ApplyDamage);
    }
}
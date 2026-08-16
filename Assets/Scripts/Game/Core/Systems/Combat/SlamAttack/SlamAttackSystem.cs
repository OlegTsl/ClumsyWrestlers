using Game.Core.Character;
using Game.Core.GameEvents;
using UnityEngine;

namespace Game.Core.Systems
{
    public sealed class SlamAttackSystem : ISlamAttackSystem
    {
        private const float CMinimumSqrMagnitude = 0.0001f;

        private readonly GameEventsBus     _events;
        private readonly ICharacterContext _context;

        public SlamAttackSystem(
            GameEventsBus     events,
            ICharacterContext context
        )
        {
            _events  = events;
            _context = context;

            _events.Subscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);
        }

        private void OnPowerAttackStarted(OnPowerAttackStartedEvent evt)
        {
            ICharacterModel attacker = _context.GetModel(evt.CharacterID);
            if (attacker == null || !attacker.Enabled)
            {
                return;
            }

            _events.Publish(new OnForceEvent(
                attacker.CharacterID, CalculateLaunchVelocity(attacker)));
        }

        private static Vector3 CalculateLaunchVelocity(ICharacterModel attacker)
        {
            PowerAttackSettings settings = attacker.Data.Combat.PowerAttack;
            Vector3 direction            = attacker.Forward;
            direction.y = 0f;

            if (direction.sqrMagnitude <= CMinimumSqrMagnitude)
                return Vector3.zero;

            direction.Normalize();
            
            float gravity = Mathf.Abs(Physics.gravity.y);
            if (gravity <= Mathf.Epsilon)
                return direction * settings.Distance;

            float verticalVelocity   = Mathf.Sqrt(2f * gravity * settings.JumpHeight);
            float flightTime         = 2f * verticalVelocity / gravity;
            float horizontalVelocity = settings.Distance / flightTime;

            return direction * horizontalVelocity + Vector3.up * verticalVelocity;
        }

        public void Dispose()
        {
            _events.Unsubscribe<OnPowerAttackStartedEvent>(OnPowerAttackStarted);
        }
    }
}

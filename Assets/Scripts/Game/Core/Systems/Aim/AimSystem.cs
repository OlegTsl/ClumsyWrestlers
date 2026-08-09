using System;
using Game.Core.Character;
using UnityEngine;

namespace Game.Core.Systems
{
    public class AimSystem : IAimSystem
    {
        private readonly ICharacterModel _model;

        public AimSystem(ICharacterModel model)
            => _model = model;
        
        public void Update()
        {
            var settings = _model.Data.Combat;
            
            Vector3 direction = _model.Forward;
            direction.y = 0;
            direction.Normalize();

            Vector3 start  = _model.Position;
            Vector3 target = start + direction * settings.PowerAttackDistance;
            target.y = start.y;
            
            float gravity          = Physics.gravity.y;            
            float verticalVelocity = Mathf.Sqrt(-2f * gravity * settings.PowerAttackJumpHeight);
            float totalTime        = 2f * verticalVelocity / Mathf.Abs(gravity);
            
            float horizontalSpeed = settings.PowerAttackDistance / totalTime;
            
            Vector3 velocity = direction * horizontalSpeed + Vector3.up * verticalVelocity;
            Vector3 position = start;
            
            float fixedDeltaTime = Time.fixedDeltaTime;
            int segments = Math.Min(Mathf.CeilToInt(totalTime / fixedDeltaTime) + 1, 60);
            
            Vector3[] points = new Vector3[segments];
            points[0] = position;
            
            int count = 1;
            for (int i = 1; i < segments; i++)
            {
                velocity.y += gravity * fixedDeltaTime;
                position += velocity * fixedDeltaTime;
                points[i] = position;
                count++;
                
                if (position.y <= target.y && velocity.y < 0)
                    break;
            }

            _model.SetAimPositionCount(count);
            _model.SetAimPositions(points);
        }
    }
}
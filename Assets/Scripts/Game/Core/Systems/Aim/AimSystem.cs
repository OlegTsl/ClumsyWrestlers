using System;
using Game.Core.Character;
using Game.Core.Data;
using UnityEngine;

namespace Game.Core.Systems
{
    public sealed class AimSystem : IAimSystem
    {
        private const int CMaxTrajectorySegments = 60;

        private readonly ICharacterModel _model;
        private readonly Vector3[] _trajectoryPoints = new Vector3[CMaxTrajectorySegments];

        public AimSystem(ICharacterModel model)
            => _model = model;

        public void Rotate(Vector2 lookDelta)
        {
            if (Mathf.Approximately(lookDelta.x, 0f))
                return;

            float rotation = lookDelta.x * CommonData.MouseSensitivity;
            _model.SetRotation(_model.Rotation * Quaternion.Euler(0f, rotation, 0f));
        }

        public void Update()
        {
            var settings = _model.Data.Combat;
            
            Vector3 direction = _model.Forward;
            direction.y = 0;
            direction.Normalize();

            Vector3 start  = _model.Position;
            PowerAttackSettings powerAttack = settings.PowerAttack;
            if (powerAttack == null)
                return;

            Vector3 target = start + direction * powerAttack.Distance;
            target.y = start.y;
            
            float gravity          = Physics.gravity.y;            
            float verticalVelocity = Mathf.Sqrt(-2f * gravity * powerAttack.JumpHeight);
            float totalTime        = 2f * verticalVelocity / Mathf.Abs(gravity);
            
            float horizontalSpeed = powerAttack.Distance / totalTime;
            
            Vector3 velocity = direction * horizontalSpeed + Vector3.up * verticalVelocity;
            Vector3 position = start;
            
            float fixedDeltaTime = Time.fixedDeltaTime;
            int segments = Math.Min(
                Mathf.CeilToInt(totalTime / fixedDeltaTime) + 1,
                CMaxTrajectorySegments);

            _trajectoryPoints[0] = position;

            int count = 1;
            for (int i = 1; i < segments; i++)
            {
                velocity.y += gravity * fixedDeltaTime;
                position += velocity * fixedDeltaTime;

                _trajectoryPoints[i] = position;
                count++;
                
                if (position.y <= target.y && velocity.y < 0)
                    break;
            }

            _model.SetAimPositions(_trajectoryPoints);
            _model.SetAimPositionCount(count);
        }
    }
}

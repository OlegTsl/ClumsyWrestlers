using System;
using Game.Core.Character;
using UnityEngine;

namespace Game.Core.Systems
{
    public sealed class AimSystem : IAimSystem
    {
        private const int MaxTrajectorySegments = 60;

        private readonly ICharacterTransformState _model;
        private readonly ICharacterView _view;
        private readonly Vector3[] _trajectoryPoints = new Vector3[MaxTrajectorySegments];

        public AimSystem(ICharacterTransformState model, ICharacterView view)
        {
            _model = model;
            _view = view;
        }

        public void Update()
        {
            PowerAttackSettings settings = _model.Data.Combat.PowerAttack;
            Vector3 direction = _model.Forward;
            direction.y = 0f;
            direction.Normalize();

            Vector3 start = _model.Position;
            Vector3 target = start + direction * settings.Distance;
            target.y = start.y;

            float gravity = Physics.gravity.y;
            float verticalVelocity = Mathf.Sqrt(-2f * gravity * settings.JumpHeight);
            float totalTime = 2f * verticalVelocity / Mathf.Abs(gravity);
            float horizontalSpeed = settings.Distance / totalTime;
            Vector3 velocity = direction * horizontalSpeed + Vector3.up * verticalVelocity;
            Vector3 position = start;
            float fixedDeltaTime = Time.fixedDeltaTime;
            int segments = Math.Min(
                Mathf.CeilToInt(totalTime / fixedDeltaTime) + 1,
                MaxTrajectorySegments);

            _trajectoryPoints[0] = position;
            int count = 1;
            for (int i = 1; i < segments; i++)
            {
                velocity.y += gravity * fixedDeltaTime;
                position += velocity * fixedDeltaTime;
                _trajectoryPoints[i] = position;
                count++;

                if (position.y <= target.y && velocity.y < 0f)
                {
                    break;
                }
            }

            _view.SetAimPositions(_trajectoryPoints, count);
        }

        public void Dispose()
        {
        }
    }
}

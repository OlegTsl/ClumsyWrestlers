using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Bots
{
    public readonly struct BotWorldState
    {
        public bool HasEnemy { get; }
        public EntityId EnemyId { get; }
        public Vector3 EnemyPosition { get; }
        public float EnemyDistanceSqr { get; }
        public bool HasLineOfSight { get; }
        public bool CanPowerAttackSafely { get; }
        public bool HasPushOpportunity { get; }
        public EntityId PushableId { get; }
        public Vector3 PushablePosition { get; }
        public Vector3 PushStagingPosition { get; }
        public float PushScore { get; }
        public bool HasEdgeDistance { get; }
        public float EdgeDistance { get; }
        public Vector3 SafePosition { get; }

        public BotWorldState(
            bool hasEnemy,
            EntityId enemyId,
            Vector3 enemyPosition,
            float enemyDistanceSqr,
            bool hasLineOfSight,
            bool canPowerAttackSafely,
            bool hasPushOpportunity,
            EntityId pushableId,
            Vector3 pushablePosition,
            Vector3 pushStagingPosition,
            float pushScore,
            bool hasEdgeDistance,
            float edgeDistance,
            Vector3 safePosition)
        {
            HasEnemy = hasEnemy;
            EnemyId = enemyId;
            EnemyPosition = enemyPosition;
            EnemyDistanceSqr = enemyDistanceSqr;
            HasLineOfSight = hasLineOfSight;
            CanPowerAttackSafely = canPowerAttackSafely;
            HasPushOpportunity = hasPushOpportunity;
            PushableId = pushableId;
            PushablePosition = pushablePosition;
            PushStagingPosition = pushStagingPosition;
            PushScore = pushScore;
            HasEdgeDistance = hasEdgeDistance;
            EdgeDistance = edgeDistance;
            SafePosition = safePosition;
        }
    }
}

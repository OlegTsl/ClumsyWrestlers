using Game.Core.Character;
using UnityEngine;

namespace Game.Core.Bots
{
    public sealed class BotUtilityEvaluator : IBotUtilityEvaluator
    {
        private readonly IBotBehaviorSettings _settings;

        public BotUtilityEvaluator(IBotBehaviorSettings settings)
            => _settings = settings;

        public BotIntent Evaluate(
            in BotWorldState world,
            ICharacterModel character)
        {
            if (world.HasEdgeDistance &&
                world.EdgeDistance < _settings.EdgeRecoveryDistance)
            {
                return BotIntent.Recover;
            }

            if (!world.HasEnemy)
            {
                return BotIntent.Idle;
            }

            float distance = Mathf.Sqrt(world.EnemyDistanceSqr);
            float simpleRange = character.Data.Combat.SimpleAttack.HitboxRange *
                                _settings.SimpleAttackDistanceMultiplier;
            BotIntent selectedIntent = BotIntent.ChaseEnemy;
            float selectedScore = _settings.ChaseWeight;

            if (world.HasPushOpportunity)
            {
                float pushScore = world.PushScore * _settings.EnvironmentWeight;
                SelectHigher(
                    BotIntent.PushEnvironment,
                    pushScore,
                    ref selectedIntent,
                    ref selectedScore);
            }

            if (world.HasLineOfSight && distance <= simpleRange)
            {
                SelectHigher(
                    BotIntent.SimpleAttack,
                    _settings.SimpleAttackWeight,
                    ref selectedIntent,
                    ref selectedScore);
            }

            if (world.HasLineOfSight &&
                world.CanPowerAttackSafely &&
                distance >= _settings.PowerAttackMinimumDistance &&
                distance <= _settings.PowerAttackMaximumDistance)
            {
                SelectHigher(
                    BotIntent.PowerAttack,
                    _settings.PowerAttackWeight,
                    ref selectedIntent,
                    ref selectedScore);
            }

            return selectedIntent;
        }

        private static void SelectHigher(
            BotIntent candidateIntent,
            float candidateScore,
            ref BotIntent selectedIntent,
            ref float selectedScore)
        {
            if (candidateScore <= selectedScore)
            {
                return;
            }

            selectedIntent = candidateIntent;
            selectedScore = candidateScore;
        }
    }
}

using System.Collections.Generic;
using Game.Core.Character;
using Game.Core.Data;
using Game.Core.Extension;
using Game.Core.Level;
using Game.Core.Level.Entities;
using Game.Core.Teams;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Core.Bots
{
    public sealed class BotPerception : IBotPerception
    {
        private const float MinimumDirectionSqrMagnitude = 0.0001f;
        private const float SightHeight = 0.75f;

        private readonly ICharacterModel _self;
        private readonly ICharacterContext _characters;
        private readonly ILevelEntityRegistry _levelEntities;
        private readonly ILevelArenaData _arena;
        private readonly ITeamRelations _teamRelations;
        private readonly IBotNavigationAgent _navigation;
        private readonly IBotBehaviorSettings _settings;

        private EntityId _currentEnemyId;
        private readonly int _obstacleLayerMask =
            (1 << 0) |
            (1 << LayerData.Ground) |
            (1 << LayerData.Environment);

        public BotPerception(
            ICharacterModel self,
            ICharacterContext characters,
            ILevelEntityRegistry levelEntities,
            ILevelArenaData arena,
            ITeamRelations teamRelations,
            IBotNavigationAgent navigation,
            IBotBehaviorSettings settings)
        {
            _self = self;
            _characters = characters;
            _levelEntities = levelEntities;
            _arena = arena;
            _teamRelations = teamRelations;
            _navigation = navigation;
            _settings = settings;
        }

        public BotWorldState Sense()
        {
            ICharacterTransformState selfTransform =
                _self.GetState<ICharacterTransformState>();
            TeamId selfTeam = _self.GetState<ICharacterTeamState>().TeamId;
            bool hasEnemy = TrySelectEnemy(
                selfTransform.Position,
                selfTeam,
                out ICharacterModel enemy,
                out float enemyDistanceSqr);

            Vector3 enemyPosition = hasEnemy
                ? enemy.GetState<ICharacterTransformState>().Position
                : Vector3.zero;
            bool hasLineOfSight = hasEnemy && HasLineOfSight(
                selfTransform.Position,
                enemyPosition);
            bool canPowerAttackSafely = hasEnemy &&
                                        CanPowerAttackSafely(
                                            selfTransform.Position,
                                            enemyPosition);
            EntityId pushableId = EntityId.Invalid;
            Vector3 pushablePosition = Vector3.zero;
            Vector3 stagingPosition = Vector3.zero;
            float pushScore = 0f;
            bool hasPushOpportunity = hasEnemy && TrySelectPushOpportunity(
                selfTransform.Position,
                enemyPosition,
                selfTeam,
                out pushableId,
                out pushablePosition,
                out stagingPosition,
                out pushScore);
            bool hasEdgeDistance = _navigation.TryGetDistanceToDropEdge(
                selfTransform.Position,
                out float edgeDistance);

            return new BotWorldState(
                hasEnemy,
                hasEnemy ? enemy.CharacterID : EntityId.Invalid,
                enemyPosition,
                enemyDistanceSqr,
                hasLineOfSight,
                canPowerAttackSafely,
                hasPushOpportunity,
                hasPushOpportunity ? pushableId : EntityId.Invalid,
                hasPushOpportunity ? pushablePosition : Vector3.zero,
                hasPushOpportunity ? stagingPosition : Vector3.zero,
                hasPushOpportunity ? pushScore : 0f,
                hasEdgeDistance,
                edgeDistance,
                _arena.SafePosition);
        }

        private bool CanPowerAttackSafely(
            Vector3 selfPosition,
            Vector3 enemyPosition)
        {
            Vector3 attackDirection = Normalize(enemyPosition - selfPosition);
            if (attackDirection == Vector3.zero)
            {
                return false;
            }

            Vector3 landingPosition = selfPosition +
                                      attackDirection *
                                      _self.Data.Combat.PowerAttack.Distance;
            return _navigation.CanStandAt(
                landingPosition,
                _settings.EdgeRecoveryDistance);
        }

        private bool TrySelectEnemy(
            Vector3 selfPosition,
            TeamId selfTeam,
            out ICharacterModel selectedEnemy,
            out float selectedDistanceSqr)
        {
            selectedEnemy = null;
            selectedDistanceSqr = _settings.PerceptionRadius *
                                  _settings.PerceptionRadius;
            float selectedComparisonDistanceSqr = selectedDistanceSqr;
            IReadOnlyList<ICharacterModel> characters = _characters.AllCharacters;

            for (int i = 0; i < characters.Count; i++)
            {
                ICharacterModel candidate = characters[i];
                if (candidate.CharacterID == _self.CharacterID ||
                    !candidate.GetState<ICharacterActivityState>().Enabled ||
                    !_teamRelations.AreEnemies(
                        selfTeam,
                        candidate.GetState<ICharacterTeamState>().TeamId))
                {
                    continue;
                }

                Vector3 candidatePosition =
                    candidate.GetState<ICharacterTransformState>().Position;
                float distanceSqr = (candidatePosition - selfPosition).sqrMagnitude;
                float comparisonDistance = distanceSqr;
                if (candidate.CharacterID == _currentEnemyId)
                {
                    comparisonDistance *= 1f - _settings.TargetSwitchBias;
                }

                if (comparisonDistance >= selectedComparisonDistanceSqr)
                {
                    continue;
                }

                selectedEnemy = candidate;
                selectedDistanceSqr = distanceSqr;
                selectedComparisonDistanceSqr = comparisonDistance;
            }

            _currentEnemyId = selectedEnemy?.CharacterID ?? EntityId.Invalid;
            return selectedEnemy != null;
        }

        private bool TrySelectPushOpportunity(
            Vector3 selfPosition,
            Vector3 enemyPosition,
            TeamId selfTeam,
            out EntityId selectedId,
            out Vector3 selectedPosition,
            out Vector3 selectedStagingPosition,
            out float selectedScore)
        {
            selectedId = EntityId.Invalid;
            selectedPosition = Vector3.zero;
            selectedStagingPosition = Vector3.zero;
            selectedScore = 0f;
            float searchRadiusSqr = _settings.PushSearchRadius *
                                    _settings.PushSearchRadius;
            float targetDistanceSqr = _settings.PushEnemyMaximumDistance *
                                      _settings.PushEnemyMaximumDistance;
            Vector3 outward = Normalize(enemyPosition - _arena.SafePosition);
            IReadOnlyList<ILevelEntityView> entities = _levelEntities.LevelEntities;

            for (int i = 0; i < entities.Count; i++)
            {
                ILevelEntityView entity = entities[i];
                if (entity is not IImpulseReceiverView ||
                    (entity.Position - selfPosition).sqrMagnitude > searchRadiusSqr)
                {
                    continue;
                }

                Vector3 pushDirection = enemyPosition - entity.Position;
                pushDirection.y = 0f;
                float pushDistanceSqr = pushDirection.sqrMagnitude;
                if (pushDistanceSqr < MinimumDirectionSqrMagnitude ||
                    pushDistanceSqr > targetDistanceSqr)
                {
                    continue;
                }

                pushDirection.Normalize();
                float alignment = Vector3.Dot(pushDirection, outward);
                if (alignment < _settings.PushRequiredOutwardAlignment ||
                    HasAllyInLane(
                        entity.Position,
                        enemyPosition,
                        selfTeam))
                {
                    continue;
                }

                float proximity = 1f - Mathf.Clamp01(
                    Mathf.Sqrt(pushDistanceSqr) /
                    _settings.PushEnemyMaximumDistance);
                float score = Mathf.Clamp01(alignment) * (0.5f + proximity * 0.5f);
                if (score <= selectedScore)
                {
                    continue;
                }

                selectedId = entity.EntityId;
                selectedPosition = entity.Position;
                selectedStagingPosition = entity.Position -
                                          pushDirection *
                                          _settings.PushStandOffDistance;
                selectedScore = score;
            }

            return selectedId.IsValid;
        }

        private bool HasAllyInLane(
            Vector3 start,
            Vector3 end,
            TeamId selfTeam)
        {
            float laneRadiusSqr = _settings.PushLaneRadius *
                                  _settings.PushLaneRadius;
            IReadOnlyList<ICharacterModel> characters = _characters.AllCharacters;
            for (int i = 0; i < characters.Count; i++)
            {
                ICharacterModel character = characters[i];
                if (character.CharacterID == _self.CharacterID ||
                    !character.GetState<ICharacterActivityState>().Enabled ||
                    !_teamRelations.AreAllies(
                        selfTeam,
                        character.GetState<ICharacterTeamState>().TeamId))
                {
                    continue;
                }

                Vector3 position =
                    character.GetState<ICharacterTransformState>().Position;
                if (DistanceToSegmentSqr(position, start, end) <= laneRadiusSqr)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasLineOfSight(Vector3 start, Vector3 end)
        {
            start.y += SightHeight;
            end.y += SightHeight;
            return !Physics.Linecast(
                start,
                end,
                _obstacleLayerMask,
                QueryTriggerInteraction.Ignore);
        }

        private static float DistanceToSegmentSqr(
            Vector3 point,
            Vector3 start,
            Vector3 end)
        {
            Vector3 segment = end - start;
            segment.y = 0f;
            Vector3 relative = point - start;
            relative.y = 0f;
            float segmentLengthSqr = segment.sqrMagnitude;
            if (segmentLengthSqr < MinimumDirectionSqrMagnitude)
            {
                return relative.sqrMagnitude;
            }

            float progress = Mathf.Clamp01(
                Vector3.Dot(relative, segment) / segmentLengthSqr);
            Vector3 closest = start + segment * progress;
            Vector3 difference = point - closest;
            difference.y = 0f;
            return difference.sqrMagnitude;
        }

        private static Vector3 Normalize(Vector3 direction)
        {
            direction.y = 0f;
            return direction.sqrMagnitude < MinimumDirectionSqrMagnitude
                ? Vector3.zero
                : direction.normalized;
        }
    }
}

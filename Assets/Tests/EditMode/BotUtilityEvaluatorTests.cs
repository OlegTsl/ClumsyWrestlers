using Game.Core.Bots;
using Game.Core.Character;
using Game.Core.Entities;
using Game.Core.Teams;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Tests.Core
{
    public sealed class BotUtilityEvaluatorTests
    {
        private BotBehaviorProfile _profile;
        private CharacterModel _character;
        private BotUtilityEvaluator _utility;

        [SetUp]
        public void SetUp()
        {
            _profile = ScriptableObject.CreateInstance<BotBehaviorProfile>();
            CharacterData data = AssetDatabase.LoadAssetAtPath<CharacterData>(
                "Assets/Data/Characters/wrestler_data.asset");
            _character = new CharacterModel(
                new EntityId(1u),
                data,
                new TeamId(1u));
            _utility = new BotUtilityEvaluator(_profile);
        }

        [TearDown]
        public void TearDown()
            => Object.DestroyImmediate(_profile);

        [Test]
        public void NearDropEdge_RecoveryHasPriority()
        {
            BotWorldState world = CreateWorld(
                enemyDistance: 3f,
                hasDropEdge: true,
                dropEdgeDistance: 0.5f,
                canPowerAttackSafely: true);

            BotIntent intent = _utility.Evaluate(world, _character);

            Assert.That(intent, Is.EqualTo(BotIntent.Recover));
        }

        [Test]
        public void PowerAttack_WithSafeLanding_IsSelected()
        {
            BotWorldState world = CreateWorld(
                enemyDistance: 3f,
                hasDropEdge: false,
                dropEdgeDistance: 0f,
                canPowerAttackSafely: true);

            BotIntent intent = _utility.Evaluate(world, _character);

            Assert.That(intent, Is.EqualTo(BotIntent.PowerAttack));
        }

        [Test]
        public void PowerAttack_WithUnsafeLanding_FallsBackToChase()
        {
            BotWorldState world = CreateWorld(
                enemyDistance: 3f,
                hasDropEdge: false,
                dropEdgeDistance: 0f,
                canPowerAttackSafely: false);

            BotIntent intent = _utility.Evaluate(world, _character);

            Assert.That(intent, Is.EqualTo(BotIntent.ChaseEnemy));
        }

        private static BotWorldState CreateWorld(
            float enemyDistance,
            bool hasDropEdge,
            float dropEdgeDistance,
            bool canPowerAttackSafely)
            => new(
                true,
                new EntityId(2u),
                Vector3.forward * enemyDistance,
                enemyDistance * enemyDistance,
                true,
                canPowerAttackSafely,
                false,
                EntityId.Invalid,
                Vector3.zero,
                Vector3.zero,
                0f,
                hasDropEdge,
                dropEdgeDistance,
                Vector3.zero);
    }
}

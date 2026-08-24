using Game.Core.Character;
using Game.Core.Entities;
using Game.Core.GameEvents;
using Game.Core.Systems;
using Game.Core.Teams;
using NUnit.Framework;
using UnityEngine;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Tests.Core
{
    public sealed class HitValidationSystemTests
    {
        private GameEventsBus _events;
        private HitValidationSystem _system;
        private CharacterModel _attacker;
        private CharacterModel _target;
        private int _validatedHitCount;

        [SetUp]
        public void SetUp()
        {
            _validatedHitCount = 0;
            _events = new GameEventsBus();
            CharacterContext characters = new();
            _attacker = CreateCharacter(1u, new TeamId(1u));
            _target = CreateCharacter(2u, new TeamId(1u));
            characters.AddCharacter(_attacker);
            characters.AddCharacter(_target);
            EmptyLevelRegistry level = new();
            _system = new HitValidationSystem(
                _events,
                characters,
                level,
                level,
                new TeamRelations());
            _events.Subscribe<OnHitValidatedEvent>(OnHitValidated);
        }

        [TearDown]
        public void TearDown()
        {
            _events.Unsubscribe<OnHitValidatedEvent>(OnHitValidated);
            _system.Dispose();
        }

        [Test]
        public void CharacterHitAgainstAlly_IsRejected()
        {
            PublishCharacterHit();

            Assert.That(_validatedHitCount, Is.Zero);
        }

        [Test]
        public void CharacterHitAgainstEnemy_IsValidated()
        {
            _target = CreateCharacter(3u, new TeamId(2u));
            CharacterContext characters = new();
            characters.AddCharacter(_attacker);
            characters.AddCharacter(_target);
            _events.Unsubscribe<OnHitValidatedEvent>(OnHitValidated);
            _system.Dispose();
            EmptyLevelRegistry level = new();
            _system = new HitValidationSystem(
                _events,
                characters,
                level,
                level,
                new TeamRelations());
            _events.Subscribe<OnHitValidatedEvent>(OnHitValidated);

            PublishCharacterHit();

            Assert.That(_validatedHitCount, Is.EqualTo(1));
        }

        [Test]
        public void HitAgainstInactiveEnemy_IsRejected()
        {
            _target.SetEnabled(false);

            PublishCharacterHit();

            Assert.That(_validatedHitCount, Is.Zero);
        }

        private void PublishCharacterHit()
            => _events.Publish(new OnHitDetectedEvent(new HitData(
                HitObjectType.Character,
                _attacker.CharacterID,
                HitObjectType.Character,
                _target.CharacterID,
                AttackType.Simple,
                Vector3.forward,
                0f,
                0f)));

        private void OnHitValidated(OnHitValidatedEvent evt)
            => _validatedHitCount++;

        private static CharacterModel CreateCharacter(uint id, TeamId teamId)
        {
            CharacterModel character = new(new EntityId(id), null, teamId);
            character.SetEnabled(true);
            return character;
        }
    }
}

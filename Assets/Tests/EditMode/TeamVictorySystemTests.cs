using Game.Core.Character;
using Game.Core.Entities;
using Game.Core.GameEvents;
using Game.Core.Systems;
using Game.Core.Teams;
using NUnit.Framework;
using EntityId = Game.Core.Entities.EntityId;

namespace Game.Tests.Core
{
    public sealed class TeamVictorySystemTests
    {
        private GameEventsBus _events;
        private CharacterContext _characters;
        private TeamVictorySystem _system;
        private CharacterModel _first;
        private CharacterModel _second;
        private int _winCount;
        private TeamId _winningTeam;

        [SetUp]
        public void SetUp()
        {
            _winCount = 0;
            _winningTeam = TeamId.Invalid;
            _events = new GameEventsBus();
            _characters = new CharacterContext();
            _first = CreateCharacter(1u, new TeamId(1u));
            _second = CreateCharacter(2u, new TeamId(2u));
            _characters.AddCharacter(_first);
            _characters.AddCharacter(_second);
            _system = new TeamVictorySystem(_events, _characters);
            _events.Subscribe<OnTeamWonEvent>(OnTeamWon);
        }

        [TearDown]
        public void TearDown()
        {
            _events.Unsubscribe<OnTeamWonEvent>(OnTeamWon);
            _system.Dispose();
        }

        [Test]
        public void LastActiveTeam_WinsOnce()
        {
            _second.SetEnabled(false);
            PublishElimination(_second);

            _system.FixedTick();
            PublishElimination(_second);
            _system.FixedTick();

            Assert.That(_winCount, Is.EqualTo(1));
            Assert.That(_winningTeam, Is.EqualTo(new TeamId(1u)));
        }

        [Test]
        public void SimultaneousElimination_DoesNotDeclareFalseWinner()
        {
            _first.SetEnabled(false);
            _second.SetEnabled(false);
            PublishElimination(_first);
            PublishElimination(_second);

            _system.FixedTick();

            Assert.That(_winCount, Is.Zero);
        }

        private void PublishElimination(CharacterModel character)
            => _events.Publish(new OnCharacterEliminatedEvent(
                character.CharacterID,
                character.TeamId));

        private void OnTeamWon(OnTeamWonEvent evt)
        {
            _winCount++;
            _winningTeam = evt.TeamId;
        }

        private static CharacterModel CreateCharacter(uint id, TeamId teamId)
        {
            CharacterModel character = new(new EntityId(id), null, teamId);
            character.SetEnabled(true);
            return character;
        }
    }
}

using Game.Core.Teams;
using NUnit.Framework;

namespace Game.Tests.Core
{
    public sealed class TeamRelationsTests
    {
        private readonly TeamRelations _relations = new();

        [Test]
        public void SameValidTeam_IsAlliedAndNotEnemy()
        {
            TeamId team = new(1u);

            Assert.That(_relations.AreAllies(team, team), Is.True);
            Assert.That(_relations.AreEnemies(team, team), Is.False);
        }

        [Test]
        public void DifferentValidTeams_AreEnemiesAndNotAllied()
        {
            TeamId first = new(1u);
            TeamId second = new(2u);

            Assert.That(_relations.AreEnemies(first, second), Is.True);
            Assert.That(_relations.AreAllies(first, second), Is.False);
        }

        [Test]
        public void InvalidTeam_HasNoRelationship()
        {
            TeamId valid = new(1u);

            Assert.That(_relations.AreAllies(TeamId.Invalid, valid), Is.False);
            Assert.That(_relations.AreEnemies(TeamId.Invalid, valid), Is.False);
        }
    }
}

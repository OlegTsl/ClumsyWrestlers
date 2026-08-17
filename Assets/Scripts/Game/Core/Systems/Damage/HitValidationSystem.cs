using Game.Core.Character;
using Game.Core.Extension;
using Game.Core.GameEvents;
using Game.Core.Level;
using Game.Core.Level.Entities;
using Game.Core.Teams;

namespace Game.Core.Systems
{
    public sealed class HitValidationSystem : IHitValidationSystem
    {
        private readonly IGameEventsBus _events;
        private readonly ICharacterContext _characters;
        private readonly ILevelEntityRegistry _levelEntities;
        private readonly ILevelImpactSettingsRegistry _impactSettings;
        private readonly ITeamRelations _teamRelations;

        public HitValidationSystem(
            IGameEventsBus events,
            ICharacterContext characters,
            ILevelEntityRegistry levelEntities,
            ILevelImpactSettingsRegistry impactSettings,
            ITeamRelations teamRelations)
        {
            _events = events;
            _characters = characters;
            _levelEntities = levelEntities;
            _impactSettings = impactSettings;
            _teamRelations = teamRelations;
            _events.Subscribe<OnHitDetectedEvent>(OnHitDetected);
        }

        private void OnHitDetected(OnHitDetectedEvent evt)
        {
            if (CanResolve(evt.Hit))
            {
                _events.Publish(new OnHitValidatedEvent(evt.Hit));
            }
        }

        private bool CanResolve(in HitData hit)
        {
            ICharacterModel sourceCharacter = null;
            if (hit.SourceType == HitObjectType.Character)
            {
                sourceCharacter = _characters.GetModel(hit.SourceID);
                if (!IsActive(sourceCharacter))
                    return false;
            }
            else if (!_impactSettings.TryGetImpactSettings(hit.SourceID, out _))
                return false;

            if (hit.TargetType == HitObjectType.Character)
            {
                ICharacterModel targetCharacter = _characters.GetModel(hit.TargetID);
                if (!IsActive(targetCharacter))
                    return false;

                return sourceCharacter == null ||
                       _teamRelations.AreEnemies(
                           sourceCharacter.GetState<ICharacterTeamState>().TeamId,
                           targetCharacter.GetState<ICharacterTeamState>().TeamId);
            }

            return _impactSettings.TryGetImpactSettings(hit.TargetID, out _) &&
                   _levelEntities.TryGetCapability(hit.TargetID, out IImpulseReceiverView _);
        }

        private static bool IsActive(ICharacterModel character)
            => character != null &&
               character.GetState<ICharacterActivityState>().Enabled;

        public void Dispose()
            => _events.Unsubscribe<OnHitDetectedEvent>(OnHitDetected);
    }
}

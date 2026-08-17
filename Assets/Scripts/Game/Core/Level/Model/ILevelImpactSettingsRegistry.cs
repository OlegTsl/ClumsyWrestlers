using Game.Core.Entities;
using Game.Core.Level.Entities;

namespace Game.Core.Level
{
    public interface ILevelImpactSettingsRegistry
    {
        bool TryGetImpactSettings(
            EntityId entityId,
            out LevelEntityImpactSettings settings);
    }
}

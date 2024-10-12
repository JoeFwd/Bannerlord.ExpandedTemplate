using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.EquipmentPools.Providers
{
    public class EncounterTypeProvider : IEncounterTypeProvider
    {
        public EncounterType GetEncounterType()
        {
            var currentMission = Mission.Current;
            if (currentMission == null) return EncounterType.None;

            if (currentMission.IsSiegeBattle || currentMission.IsSallyOutBattle) return EncounterType.Siege;
            if (currentMission.IsFriendlyMission) return EncounterType.Civilian;
            return EncounterType.Battle;
        }
    }
}
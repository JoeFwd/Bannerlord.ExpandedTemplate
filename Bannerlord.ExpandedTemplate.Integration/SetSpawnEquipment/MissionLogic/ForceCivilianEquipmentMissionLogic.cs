using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic
{
    public class ForceCivilianEquipmentMissionLogic : TaleWorlds.MountAndBlade.MissionLogic
    {
        public override void OnAgentCreated(Agent agent)
        {
            CampaignMission.Current?.Location?.GetCharacterList()?.ToList().ForEach(locationCharacter =>
                locationCharacter.AgentData.CivilianEquipment(true) 
            );
        }
    }
}
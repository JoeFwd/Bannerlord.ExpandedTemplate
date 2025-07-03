using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic.EquipmentSetters;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic;

public class EquipmentSetterMissionLogic : TaleWorlds.MountAndBlade.MissionLogic
{
    private readonly ILogger _logger;
    private readonly HeroEquipmentSetter _heroEquipmentSetter;
    private readonly TroopEquipmentPoolSetter _troopEquipmentPoolSetter;
    private readonly IGetEquipmentPool _getEquipmentPool;
    private readonly CharacterEquipmentRosterReference _characterEquipmentRosterReference;

    private readonly Dictionary<string, MBEquipmentRoster> _nativeTroopEquipmentRosters = new();
    private readonly Dictionary<string, Equipment> _nativeHeroEquipment = new();

    public EquipmentSetterMissionLogic(HeroEquipmentSetter heroEquipmentSetter,
        TroopEquipmentPoolSetter troopEquipmentPoolSetter, IGetEquipmentPool getEquipmentPool,
        CharacterEquipmentRosterReference characterEquipmentRosterReference, ILoggerFactory loggerFactory)
    {
        _heroEquipmentSetter = heroEquipmentSetter;
        _troopEquipmentPoolSetter = troopEquipmentPoolSetter;
        _getEquipmentPool = getEquipmentPool;
        _characterEquipmentRosterReference = characterEquipmentRosterReference;
        _logger = loggerFactory.CreateLogger<EquipmentSetterMissionLogic>();
    }

    public override void OnBehaviorInitialize()
    {
        base.OnBehaviorInitialize();

        _nativeTroopEquipmentRosters.Clear();
        _nativeHeroEquipment.Clear();
    }

    public override void OnAgentCreated(Agent agent)
    {
        MBEquipmentRoster? agentEquipmentRoster =
            _characterEquipmentRosterReference.GetEquipmentRoster(agent.Character);

        if (agentEquipmentRoster is null) return;
        if (!CanOverrideEquipment(agent)) return;

        base.OnAgentCreated(agent);

        string id = agent.Character.StringId;
        if (agent.Character is CharacterObject characterObject)
            id = characterObject.OriginalCharacter?.StringId ?? id;

        var equipmentPool = _getEquipmentPool.GetTroopEquipmentPool(id);
        if (equipmentPool.IsEmpty())
            equipmentPool = _getEquipmentPool.GetTroopEquipmentPool(agentEquipmentRoster.StringId);

        if (agent.IsHero)
        {
            _nativeHeroEquipment[agent.Character.StringId] = agent.Character.Equipment.Clone();
            _heroEquipmentSetter.SetEquipmentFromEquipmentPool(agent, equipmentPool);
        }
        else
        {
            _nativeTroopEquipmentRosters[agent.Character.StringId] = agentEquipmentRoster;
            _troopEquipmentPoolSetter.SetEquipmentPool(agent, equipmentPool);
        }
    }

    public override void OnAgentBuild(Agent agent, Banner banner)
    {
        if (!CanOverrideEquipment(agent)) return;

        base.OnAgentBuild(agent, banner);

        if (agent.IsHero)
        {
            _heroEquipmentSetter.SetEquipment(agent, _nativeHeroEquipment[agent.Character.StringId]);
        }
        else
        {
            _troopEquipmentPoolSetter.SetEquipmentPool(agent,
                _nativeTroopEquipmentRosters[agent.Character.StringId]);
        }

        if (agent.SpawnEquipment.IsEmpty())
            _logger.Warn(
                $"Troop '{agent.Character.Name.Value}' with id '{agent.Character.StringId}' spawned with no equipment.");
    }

    private static bool CanOverrideEquipment(IAgent agent)
    {
        return (Mission.Current?.GetMissionBehavior<MissionAgentHandler>() is not null ||
                Mission.Current?.GetMissionBehavior<IMissionAgentSpawnLogic>() is not null) &&
               agent?.Character is not null &&
               !(Clan.PlayerClan?.Heroes?.Exists(
                   hero => hero?.StringId is not null && agent?.Character?.StringId == hero.StringId) ?? true);
    }
}
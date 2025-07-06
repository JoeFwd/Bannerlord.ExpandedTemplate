using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.Mappers;
using TaleWorlds.Core;

namespace Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic.EquipmentSetters;

public class TroopEquipmentPoolSetter
{
    private readonly EquipmentPoolsMapper _equipmentPoolsMapper;
    private readonly CharacterEquipmentRosterReference _characterEquipmentRosterReference;

    public TroopEquipmentPoolSetter(EquipmentPoolsMapper equipmentPoolsMapper,
        CharacterEquipmentRosterReference characterEquipmentRosterReference)

    {
        _equipmentPoolsMapper = equipmentPoolsMapper;
        _characterEquipmentRosterReference = characterEquipmentRosterReference;
    }

    public void SetEquipmentPool(IAgent agent, Domain.EquipmentPool.Model.EquipmentPool equipmentPool)
    {
        if (agent?.Character is null) return;

        MBEquipmentRoster? agentEquipmentRoster =
            _characterEquipmentRosterReference.GetEquipmentRoster(agent.Character);

        if (agentEquipmentRoster is null) return;

        SetEquipmentPool(agent, _equipmentPoolsMapper.MapEquipmentPool(equipmentPool, agentEquipmentRoster));
    }

    public void SetEquipmentPool(IAgent agent, MBEquipmentRoster equipmentRoster)
    {
        if (agent?.Character is null) return;
        _characterEquipmentRosterReference.SetEquipmentRoster(agent.Character, equipmentRoster);
    }
}
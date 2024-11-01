using Bannerlord.ExpandedTemplate.Domain.EquipmentPool;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.EquipmentPools.Mappers;
using TaleWorlds.Core;
using Equipment = TaleWorlds.Core.Equipment;

namespace Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic.EquipmentSetters;

public class HeroEquipmentSetter
{
    private readonly IGetEquipment _getEquipment;
    private readonly EquipmentMapper _equipmentMapper;
    private readonly CharacterEquipmentRosterReference _characterEquipmentRosterReference;
    private readonly ILogger _logger;

    public HeroEquipmentSetter(IGetEquipment getEquipment, EquipmentMapper equipmentMapper,
        CharacterEquipmentRosterReference characterEquipmentRosterReference, ILoggerFactory loggerFactory)
    {
        _getEquipment = getEquipment;
        _equipmentMapper = equipmentMapper;
        _characterEquipmentRosterReference = characterEquipmentRosterReference;
        _logger = loggerFactory.CreateLogger<HeroEquipmentSetter>();
    }

    public void SetEquipmentFromEquipmentPool(IAgent agent, EquipmentPool equipmentPool)
    {
        MBEquipmentRoster? agentEquipmentRoster =
            _characterEquipmentRosterReference.GetEquipmentRoster(agent.Character);

        if (agentEquipmentRoster is null) return;

        var equipment = _getEquipment.GetEquipmentFromEquipmentPool(equipmentPool);
        if (equipment is null)
            SetEquipment(agent, new Equipment());
        else
            SetEquipment(agent,
                _equipmentMapper.Map(equipment, agentEquipmentRoster));
    }

    public void SetEquipment(IAgent agent, Equipment? equipment)
    {
        if (agent?.Character?.Equipment is null)
        {
            _logger.Error(
                "Expected a hero Agent to have a non-nullable Character field with a non-nullable Equipment field");
            return;
        }

        if (equipment is null)
        {
            _logger.Error(
                $"Could find any equipment for ${agent.Character.StringId}");
            return;
        }

        agent.Character.Equipment.FillFrom(equipment);
    }
}
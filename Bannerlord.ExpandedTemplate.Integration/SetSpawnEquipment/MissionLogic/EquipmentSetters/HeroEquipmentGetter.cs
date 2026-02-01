using Bannerlord.ExpandedTemplate.Domain.EquipmentPool;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.Mappers;
using TaleWorlds.Core;
using Equipment = TaleWorlds.Core.Equipment;

namespace Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic.EquipmentSetters;

public class HeroEquipmentGetter
{
    private readonly IGetEquipment _getEquipment;
    private readonly EquipmentMapper _equipmentMapper;
    private readonly CharacterEquipmentRosterReference _characterEquipmentRosterReference;
    private readonly ILogger _logger;

    public HeroEquipmentGetter(IGetEquipment getEquipment, EquipmentMapper equipmentMapper,
        CharacterEquipmentRosterReference characterEquipmentRosterReference, ILoggerFactory loggerFactory)
    {
        _getEquipment = getEquipment;
        _equipmentMapper = equipmentMapper;
        _characterEquipmentRosterReference = characterEquipmentRosterReference;
        _logger = loggerFactory.CreateLogger<HeroEquipmentGetter>();
    }

    public Equipment GetEquipmentFromEquipmentPool(BasicCharacterObject characterObject,
        Domain.EquipmentPool.Model.EquipmentPool equipmentPool)
    {
        var equipment = _getEquipment.GetEquipmentFromEquipmentPool(equipmentPool);
        if (equipment is null)
            return new Equipment();
        return _equipmentMapper.Map(equipment, _characterEquipmentRosterReference.GetEquipmentRoster(characterObject));
    }
}

using System.Linq;
using System.Reflection;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using TaleWorlds.Core;
using TaleWorlds.Library;
using Equipment = TaleWorlds.Core.Equipment;

namespace Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.EquipmentPools.Mappers;

public class EquipmentPoolsMapper
{
    private readonly EquipmentMapper _equipmentMapper;

    private readonly FieldInfo? _mbEquipmentRosterEquipmentsField =
        typeof(MBEquipmentRoster).GetField("_equipments", BindingFlags.NonPublic | BindingFlags.Instance);

    public EquipmentPoolsMapper(EquipmentMapper equipmentMapper, ILoggerFactory loggerFactory)
    {
        ILogger logger = loggerFactory.CreateLogger<EquipmentMapper>();
        if (_mbEquipmentRosterEquipmentsField is null)
            logger.Error("Could not find the _equipment field in the MBEquipmentRoster class via reflection.");
        _equipmentMapper = equipmentMapper;
    }

    public MBEquipmentRoster MapEquipmentPool(EquipmentPool equipmentPool,
        MBEquipmentRoster equipmentPoolWithAllEquipment)
    {
        if (_mbEquipmentRosterEquipmentsField is null) return new MBEquipmentRoster();

        var mbEquipmentLoadouts = new MBEquipmentRoster();
        var equipments = (MBList<Equipment>)_mbEquipmentRosterEquipmentsField.GetValue(mbEquipmentLoadouts);

        equipmentPool.GetEquipmentLoadouts().ToList().ForEach(equipment =>
            equipments.Add(_equipmentMapper.Map(equipment, equipmentPoolWithAllEquipment)));

        return mbEquipmentLoadouts;
    }
}
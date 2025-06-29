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

    private readonly FieldInfo? _equipmentsFieldInfo =
        typeof(MBEquipmentRoster).GetField("_equipments", BindingFlags.NonPublic | BindingFlags.Instance);

    public EquipmentPoolsMapper(EquipmentMapper equipmentMapper, ILoggerFactory loggerFactory)
    {
        ILogger logger = loggerFactory.CreateLogger<EquipmentMapper>();
        if (_equipmentsFieldInfo is null)
            logger.Error("Could not find the '_equipments' field in the MBEquipmentRoster class via reflection.");
        _equipmentMapper = equipmentMapper;
    }

    /// <summary>
    ///     Maps and duplicates the equipment pool's loadouts to an MBEquipmentRoster.
    ///     Duplicates the equipment with different civilian/battle flags.
    /// </summary>
    /// <param name="sourceEquipmentPool">The equipment pool to map from.</param>
    /// <param name="equipmentRosterTemplate">A template for mapping the equipment pool.</param>
    /// <returns>An MBEquipmentRoster containing the duplicated loadouts with battle and civilian flag.</returns>
    public MBEquipmentRoster MapEquipmentPool(EquipmentPool sourceEquipmentPool,
        MBEquipmentRoster equipmentRosterTemplate)
    {
        if (_equipmentsFieldInfo is null) return new MBEquipmentRoster();

        var resultEquipmentRoster = new MBEquipmentRoster();
        var equipmentList = (MBList<Equipment>)_equipmentsFieldInfo.GetValue(resultEquipmentRoster);

        var primaryLoadouts = sourceEquipmentPool.GetEquipmentLoadouts()
            .Select(equipment => _equipmentMapper.Map(equipment, equipmentRosterTemplate))
            .ToList();

        var secondaryLoadouts = primaryLoadouts
            .Select(equipmentLoadout => CloneEquipment(equipmentLoadout, !equipmentLoadout.IsCivilian))
            .ToList();

        // Add equipment twice but with different flags (battle/civilian).
        // Bannerlord filters out civilian/battle equipment depending on the IsCivilian flag => Equipment.GetRandomEquipmentElements.
        // We've prepared the equipment for a specific scenario type, and we want Bannerlord to use our equipment irrespective of that flag.
        // Therefore, we duplicate the equipment so it can be utilized by Bannerlord in any native scenario (battle/civilian).
        equipmentList.AddRange(primaryLoadouts);
        equipmentList.AddRange(secondaryLoadouts);

        return resultEquipmentRoster;
    }

    private Equipment CloneEquipment(Equipment originalEquipment, bool isCivilian)
    {
        Equipment clonedEquipment = new Equipment(isCivilian);
        clonedEquipment.FillFrom(originalEquipment, false);
        return clonedEquipment;
    }
}
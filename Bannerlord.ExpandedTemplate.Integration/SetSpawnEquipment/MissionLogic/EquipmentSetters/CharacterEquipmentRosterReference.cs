using System.Reflection;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using TaleWorlds.Core;

namespace Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic.EquipmentSetters;

public class CharacterEquipmentRosterReference
{
    private static readonly FieldInfo? EquipmentRosterField =
        typeof(BasicCharacterObject).GetField("_equipmentRoster", BindingFlags.NonPublic | BindingFlags.Instance)!;

    public CharacterEquipmentRosterReference(ILoggerFactory loggerFactory)
    {
        ILogger logger = loggerFactory.CreateLogger<CharacterEquipmentRosterReference>();

        if (EquipmentRosterField is null || EquipmentRosterField.FieldType != typeof(MBEquipmentRoster))
            logger.Error(
                "BasicCharacterObject's _mbEquipmentRoster field could not be found preventing equipment pool override");
    }

    /// <summary>
    ///     Returns the equipment roster reference of a characterObject
    /// </summary>
    /// <param name="characterObject"></param>
    /// <returns>
    ///     the internal MBEquipmentRoster object from the characterObject object or
    ///     null if the characterObject is null or if the under the hood reflection implementation fails due to
    ///     a game update)
    /// </returns>
    public MBEquipmentRoster? GetEquipmentRoster(BasicCharacterObject characterObject)
    {
        if (characterObject is null) return null;
        return (MBEquipmentRoster?)EquipmentRosterField?.GetValue(characterObject);
    }

    /// <summary>
    ///     Sets the equipment roster reference of a characterObject
    /// </summary>
    /// <param name="characterObject"></param>
    /// <param name="mbEquipmentRoster"></param>
    public void SetEquipmentRoster(BasicCharacterObject characterObject, MBEquipmentRoster mbEquipmentRoster)
    {
        if (characterObject is null) return;
        EquipmentRosterField?.SetValue(characterObject, mbEquipmentRoster);
    }
}
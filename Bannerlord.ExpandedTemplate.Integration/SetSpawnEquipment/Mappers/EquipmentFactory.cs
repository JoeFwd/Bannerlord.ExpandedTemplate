using TaleWorlds.Core;

namespace Bannerlord.ExpandedTemplate.Integration;

public class EquipmentFactory
{
    /// <summary>
    ///     Creates a new Equipment instance with the specified EquipmentType.
    /// </summary>
    /// <param name="equipmentType">The type of equipment to create</param>
    /// <returns>A new Equipment instance</returns>
    public Equipment CreateEquipment(Equipment.EquipmentType equipmentType)
    {
        return new Equipment(Equipment.EquipmentType.Civilian.Equals(equipmentType));
    }
}
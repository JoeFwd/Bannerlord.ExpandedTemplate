using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Util;

namespace Bannerlord.ExpandedTemplate.Domain.EquipmentPool;

public class GetEquipment(IRandom random) : IGetEquipment
{
    public Equipment GetEquipmentFromEquipmentPool(Model.EquipmentPool equipmentPool)
    {
        if (equipmentPool is null || equipmentPool.GetEquipmentLoadouts().Count == 0) return null;

        var randomIndex = random.Next(0, equipmentPool.GetEquipmentLoadouts().Count);

        return equipmentPool.GetEquipmentLoadouts()[randomIndex];
    }
}
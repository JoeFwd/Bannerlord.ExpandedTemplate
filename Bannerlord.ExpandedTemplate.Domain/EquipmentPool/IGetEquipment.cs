using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;

namespace Bannerlord.ExpandedTemplate.Domain.EquipmentPool;

public interface IGetEquipment
{
    Equipment? GetEquipmentFromEquipmentPool(Model.EquipmentPool equipmentPool);
}
using System.Collections.Generic;

namespace Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Util
{
    public interface IEquipmentPoolPicker
    {
        Model.EquipmentPool PickEquipmentPool(IList<Model.EquipmentPool> equipmentPools);
    }
}
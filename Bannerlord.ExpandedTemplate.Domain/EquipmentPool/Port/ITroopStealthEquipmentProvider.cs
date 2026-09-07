using System.Collections.Generic;

namespace Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port
{
    public interface ITroopStealthEquipmentProvider
    {
        IList<Model.EquipmentPool> GetStealthTroopEquipmentPools(string equipmentId);
    }
}

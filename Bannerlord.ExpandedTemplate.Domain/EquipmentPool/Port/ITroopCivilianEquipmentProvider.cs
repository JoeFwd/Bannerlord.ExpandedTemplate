using System.Collections.Generic;

namespace Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port
{
    public interface ITroopCivilianEquipmentProvider
    {
        IList<Model.EquipmentPool> GetCivilianTroopEquipmentPools(string equipmentId);
    }
}
using System.Collections.Generic;

namespace Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port
{
    public interface ITroopSiegeEquipmentProvider
    {
        IList<Model.EquipmentPool> GetSiegeTroopEquipmentPools(string equipmentId);
    }
}
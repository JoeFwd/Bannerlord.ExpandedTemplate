using System.Collections.Generic;

namespace Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port
{
    public interface ITroopBattleEquipmentProvider
    {
        IList<Model.EquipmentPool> GetBattleTroopEquipmentPools(string equipmentId);
    }
}
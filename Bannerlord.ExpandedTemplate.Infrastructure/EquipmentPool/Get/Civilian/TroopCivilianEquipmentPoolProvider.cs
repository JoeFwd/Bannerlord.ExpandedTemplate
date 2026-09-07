using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Civilian;

public class TroopCivilianEquipmentPoolProvider : ITroopCivilianEquipmentProvider
{
    private readonly TroopEquipmentPoolProvider _troopEquipmentPoolProvider;

    public TroopCivilianEquipmentPoolProvider(ILoggerFactory loggerFactory, IEquipmentPoolsProvider equipmentPoolsProvider)
    {
        _troopEquipmentPoolProvider = new TroopEquipmentPoolProvider(
            loggerFactory.CreateLogger<TroopCivilianEquipmentPoolProvider>(), equipmentPoolsProvider, "civilian");
    }

    public IList<Domain.EquipmentPool.Model.EquipmentPool> GetCivilianTroopEquipmentPools(string equipmentId)
    {
        return _troopEquipmentPoolProvider.GetTroopEquipmentPools(equipmentId);
    }
}

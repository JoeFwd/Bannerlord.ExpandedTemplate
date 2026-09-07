using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Siege;

public class TroopSiegeEquipmentPoolProvider : ITroopSiegeEquipmentProvider
{
    private readonly TroopEquipmentPoolProvider _troopEquipmentPoolProvider;

    public TroopSiegeEquipmentPoolProvider(ILoggerFactory loggerFactory, IEquipmentPoolsProvider equipmentPoolsProvider)
    {
        _troopEquipmentPoolProvider = new TroopEquipmentPoolProvider(
            loggerFactory.CreateLogger<TroopSiegeEquipmentPoolProvider>(), equipmentPoolsProvider, "siege");
    }

    public IList<Domain.EquipmentPool.Model.EquipmentPool> GetSiegeTroopEquipmentPools(string equipmentId)
    {
        return _troopEquipmentPoolProvider.GetTroopEquipmentPools(equipmentId);
    }
}

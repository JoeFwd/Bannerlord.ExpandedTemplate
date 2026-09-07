using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Stealth;

public class TroopStealthEquipmentPoolProvider : ITroopStealthEquipmentProvider
{
    private readonly TroopEquipmentPoolProvider _troopEquipmentPoolProvider;

    public TroopStealthEquipmentPoolProvider(ILoggerFactory loggerFactory, IEquipmentPoolsProvider equipmentPoolsProvider)
    {
        _troopEquipmentPoolProvider = new TroopEquipmentPoolProvider(
            loggerFactory.CreateLogger<TroopStealthEquipmentPoolProvider>(), equipmentPoolsProvider, "stealth");
    }

    public IList<Domain.EquipmentPool.Model.EquipmentPool> GetStealthTroopEquipmentPools(string equipmentId)
    {
        return _troopEquipmentPoolProvider.GetTroopEquipmentPools(equipmentId);
    }
}

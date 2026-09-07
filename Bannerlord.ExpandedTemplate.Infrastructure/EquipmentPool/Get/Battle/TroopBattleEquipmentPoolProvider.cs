using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Battle;

public class TroopBattleEquipmentPoolProvider : ITroopBattleEquipmentProvider
{
    private readonly TroopEquipmentPoolProvider _troopEquipmentPoolProvider;

    public TroopBattleEquipmentPoolProvider(ILoggerFactory loggerFactory, IEquipmentPoolsProvider equipmentPoolsProvider)
    {
        _troopEquipmentPoolProvider = new TroopEquipmentPoolProvider(
            loggerFactory.CreateLogger<TroopBattleEquipmentPoolProvider>(), equipmentPoolsProvider, "battle");
    }

    public IList<Domain.EquipmentPool.Model.EquipmentPool> GetBattleTroopEquipmentPools(string equipmentId)
    {
        return _troopEquipmentPoolProvider.GetTroopEquipmentPools(equipmentId);
    }
}

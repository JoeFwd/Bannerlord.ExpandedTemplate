using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Siege;

public class TroopSiegeEquipmentPoolProvider : ITroopSiegeEquipmentProvider
{
    private readonly ILogger _logger;
    private readonly IEquipmentPoolsProvider _equipmentPoolsProvider;

    public TroopSiegeEquipmentPoolProvider(ILoggerFactory loggerFactory,
        IEquipmentPoolsProvider equipmentPoolsProvider)
    {
        _logger = loggerFactory.CreateLogger<TroopSiegeEquipmentPoolProvider>();
        _equipmentPoolsProvider = equipmentPoolsProvider;
    }

    public IList<Domain.EquipmentPool.Model.EquipmentPool> GetSiegeTroopEquipmentPools(string equipmentId)
    {
        if (string.IsNullOrWhiteSpace(equipmentId))
        {
            _logger.Debug("The equipment id is null or empty.");
            return new List<Domain.EquipmentPool.Model.EquipmentPool>();
        }

        var troopEquipmentPools = _equipmentPoolsProvider.GetEquipmentPoolsByCharacterId();
        if (!troopEquipmentPools.ContainsKey(equipmentId))
        {
            _logger.Warn($"The equipment id {equipmentId} is not in the siege equipment pools.");
            return new List<Domain.EquipmentPool.Model.EquipmentPool>();
        }

        return troopEquipmentPools[equipmentId];
    }
}
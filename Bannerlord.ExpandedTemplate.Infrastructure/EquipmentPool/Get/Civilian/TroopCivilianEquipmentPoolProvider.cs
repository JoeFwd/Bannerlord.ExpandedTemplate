using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Civilian;

public class TroopCivilianEquipmentPoolProvider : ITroopCivilianEquipmentProvider
{
    private readonly ILogger _logger;
    private readonly IEquipmentPoolsProvider _equipmentPoolsProvider;

    public TroopCivilianEquipmentPoolProvider(ILoggerFactory loggerFactory,
        IEquipmentPoolsProvider equipmentPoolsProvider)
    {
        _logger = loggerFactory.CreateLogger<TroopCivilianEquipmentPoolProvider>();
        _equipmentPoolsProvider = equipmentPoolsProvider;
    }

    public IList<Domain.EquipmentPool.Model.EquipmentPool> GetCivilianTroopEquipmentPools(string equipmentId)
    {
        if (string.IsNullOrWhiteSpace(equipmentId))
        {
            _logger.Debug("The equipment id is null or empty.");
            return new List<Domain.EquipmentPool.Model.EquipmentPool>();
        }

        var troopEquipmentPools = _equipmentPoolsProvider.GetEquipmentPoolsByCharacterId();
        if (!troopEquipmentPools.ContainsKey(equipmentId))
        {
            _logger.Warn($"The equipment id {equipmentId} is not in the civilian equipment pools.");
            return new List<Domain.EquipmentPool.Model.EquipmentPool>();
        }

        return troopEquipmentPools[equipmentId];
    }
}
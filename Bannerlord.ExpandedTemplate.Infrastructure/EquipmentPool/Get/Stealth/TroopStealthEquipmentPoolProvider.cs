using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Stealth;

public class TroopStealthEquipmentPoolProvider : ITroopStealthEquipmentProvider
{
    private readonly ILogger _logger;
    private readonly IEquipmentPoolsProvider _stealthEquipmentPoolsProvider;

    public TroopStealthEquipmentPoolProvider(ILoggerFactory loggerFactory,
        IEquipmentPoolsProvider stealthEquipmentPoolsProvider)
    {
        _logger = loggerFactory.CreateLogger<TroopStealthEquipmentPoolProvider>();
        _stealthEquipmentPoolsProvider = stealthEquipmentPoolsProvider;
    }

    public IList<Domain.EquipmentPool.Model.EquipmentPool> GetStealthTroopEquipmentPools(string equipmentId)
    {
        if (string.IsNullOrWhiteSpace(equipmentId))
        {
            _logger.Debug("The equipment id is null or empty.");
            return new List<Domain.EquipmentPool.Model.EquipmentPool>();
        }

        var troopEquipmentPools = _stealthEquipmentPoolsProvider.GetEquipmentPoolsByCharacterId();
        if (!troopEquipmentPools.ContainsKey(equipmentId))
        {
            _logger.Warn($"The equipment id {equipmentId} is not in the stealth equipment pools.");
            return new List<Domain.EquipmentPool.Model.EquipmentPool>();
        }

        return troopEquipmentPools[equipmentId];
    }
}

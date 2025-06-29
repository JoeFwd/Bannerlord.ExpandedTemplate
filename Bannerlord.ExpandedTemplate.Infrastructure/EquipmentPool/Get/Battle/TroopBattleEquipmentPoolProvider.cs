using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Battle;

public class TroopBattleEquipmentPoolProvider : ITroopBattleEquipmentProvider
{
    private readonly ILogger _logger;
    private readonly IEquipmentPoolsProvider _battleEquipmentPoolsProvider;

    public TroopBattleEquipmentPoolProvider(ILoggerFactory loggerFactory,
        IEquipmentPoolsProvider battleEquipmentPoolsProvider)
    {
        _logger = loggerFactory.CreateLogger<TroopBattleEquipmentPoolProvider>();
        _battleEquipmentPoolsProvider = battleEquipmentPoolsProvider;
    }

    public IList<Domain.EquipmentPool.Model.EquipmentPool> GetBattleTroopEquipmentPools(string equipmentId)
    {
        if (string.IsNullOrWhiteSpace(equipmentId))
        {
            _logger.Debug("The equipment id is null or empty.");
            return new List<Domain.EquipmentPool.Model.EquipmentPool>();
        }

        var troopEquipmentPools = _battleEquipmentPoolsProvider.GetEquipmentPoolsByCharacterId();
        if (!troopEquipmentPools.ContainsKey(equipmentId))
        {
            _logger.Warn($"The equipment id {equipmentId} is not in the battle equipment pools.");
            return new List<Domain.EquipmentPool.Model.EquipmentPool>();
        }

        return troopEquipmentPools[equipmentId];
    }
}
using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;
using EquipmentPoolModel = Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model.EquipmentPool;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get;

public class TroopEquipmentPoolProvider
{
    private readonly IEquipmentPoolsProvider _equipmentPoolsProvider;
    private readonly string _equipmentPoolType;
    private readonly ILogger _logger;

    public TroopEquipmentPoolProvider(ILogger logger, IEquipmentPoolsProvider equipmentPoolsProvider,
        string equipmentPoolType)
    {
        _logger = logger;
        _equipmentPoolsProvider = equipmentPoolsProvider;
        _equipmentPoolType = equipmentPoolType;
    }

    public IList<EquipmentPoolModel> GetTroopEquipmentPools(string equipmentId)
    {
        if (string.IsNullOrWhiteSpace(equipmentId))
        {
            _logger.Debug("The equipment id is null or empty.");
            return new List<EquipmentPoolModel>();
        }

        IDictionary<string, IList<EquipmentPoolModel>> troopEquipmentPools =
            _equipmentPoolsProvider.GetEquipmentPoolsByCharacterId();
        if (!troopEquipmentPools.ContainsKey(equipmentId))
        {
            _logger.Warn($"The equipment id {equipmentId} is not in the {_equipmentPoolType} equipment pools.");
            return new List<EquipmentPoolModel>();
        }

        return troopEquipmentPools[equipmentId];
    }
}

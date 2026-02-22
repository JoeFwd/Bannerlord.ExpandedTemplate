using System;
using System.Linq;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;

namespace Bannerlord.ExpandedTemplate.Domain.EquipmentPool;

public class EquipmentComparison : IEquipmentComparison
{
    private readonly IGetEquipmentPoolsUtil _getEquipmentPoolsUtil;
    private readonly ILogger _logger;

    public EquipmentComparison(IGetEquipmentPoolsUtil getEquipmentPoolsUtil, ILoggerFactory loggerFactory)
    {
        _getEquipmentPoolsUtil = getEquipmentPoolsUtil ?? throw new ArgumentNullException(nameof(getEquipmentPoolsUtil));
        _logger = loggerFactory?.CreateLogger<EquipmentComparison>() ??
                  throw new ArgumentNullException(nameof(loggerFactory));
    }

    public bool ShouldOverrideEquipment(Equipment currentEquipment, string troopId)
    {
        if (currentEquipment == null)
        {
            _logger.Warn("Current equipment is null, cannot perform comparison");
            return false;
        }

        try
        {
            var equipmentPools = _getEquipmentPoolsUtil.GetEquipmentPools(troopId);

            if (equipmentPools == null || !equipmentPools.Any()) return false;

            var allEquipmentLoadouts = equipmentPools.SelectMany(pool => pool.GetEquipmentLoadouts()).ToList();


            foreach (var poolEquipment in allEquipmentLoadouts)
                if (poolEquipment.Equals(currentEquipment))
                    return true;

            return false;
        }
        catch (Exception ex)
        {
            _logger.Error($"Error during equipment comparison: {ex.Message}");
            return false;
        }
    }
}
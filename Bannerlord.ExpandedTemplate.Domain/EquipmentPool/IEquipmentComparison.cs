using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;

namespace Bannerlord.ExpandedTemplate.Domain.EquipmentPool;

public interface IEquipmentComparison
{
    bool ShouldOverrideEquipment(Equipment currentEquipment, string troopId);
}
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories
{
    public interface IEquipmentRosterRepository
    {
        EquipmentRosters GetEquipmentRosters();
    }
}
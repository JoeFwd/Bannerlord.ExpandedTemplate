using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;

public interface IEquipmentRosterMapper
{
    Domain.EquipmentPool.Model.EquipmentPool MapToEquipmentPool(EquipmentRoster equipmentRoster);
}
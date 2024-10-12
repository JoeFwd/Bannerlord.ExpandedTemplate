using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using EquipmentSet = Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters.EquipmentSet;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;

public interface IEquipmentSetMapper
{
    EquipmentRoster MapToEquipmentRoster(EquipmentSet equipmentSet);
}
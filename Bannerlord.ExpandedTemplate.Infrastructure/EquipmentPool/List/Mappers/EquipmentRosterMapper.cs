using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Equipment = Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model.Equipment;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;

public class EquipmentRosterMapper : IEquipmentRosterMapper
{
    public Domain.EquipmentPool.Model.EquipmentPool MapToEquipmentPool(EquipmentRoster equipmentRoster)
    {
        return new Domain.EquipmentPool.Model.EquipmentPool(new List<Equipment>
        {
            new(equipmentRoster.Equipment
                .Select(equipmentSlot => new EquipmentSlot(equipmentSlot.Slot ?? "", equipmentSlot.Id ?? "")).ToList())
        }, int.TryParse(equipmentRoster.Pool, out int pool) ? pool : 0);
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
                .Select(equipmentSlot =>
                    new EquipmentSlot(equipmentSlot.Slot ?? "", ParseItemId(equipmentSlot.Id ?? ""))).ToList())
        }, int.TryParse(equipmentRoster.Pool, out int pool) ? pool : 0);
    }

    private string ParseItemId(string id)
    {
        string pattern = @"^(Item\.)?(.*)$";

        Match match = Regex.Match(id, pattern);

        return match.Success ? match.Groups[2].Value : id;
    }
}
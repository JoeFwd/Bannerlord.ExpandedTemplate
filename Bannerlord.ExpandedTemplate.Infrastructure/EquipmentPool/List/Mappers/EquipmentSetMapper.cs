using System;
using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using EquipmentSet = Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters.EquipmentSet;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;

public class EquipmentSetMapper : IEquipmentSetMapper
{
    public EquipmentRoster MapToEquipmentRoster(EquipmentSet equipmentSet)
    {
        if (equipmentSet == null) throw new ArgumentNullException(nameof(equipmentSet));

        return new EquipmentRoster
        {
            IsBattle = equipmentSet.IsBattle,
            IsCivilian = equipmentSet.IsCivilian,
            IsSiege = equipmentSet.IsSiege,
            Pool = equipmentSet.Pool,
            Equipment = equipmentSet.Equipment?.Select(equipment => new Equipment
            {
                Id = equipment.Id,
                Slot = equipment.Slot
            }).ToList() ?? new List<Equipment>()
        };
    }
}
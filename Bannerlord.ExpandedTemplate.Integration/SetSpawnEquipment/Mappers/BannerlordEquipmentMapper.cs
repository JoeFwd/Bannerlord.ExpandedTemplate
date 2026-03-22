using System;
using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using TaleWorlds.Core;
using Equipment = Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model.Equipment;

namespace Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.Mappers;

public class BannerlordEquipmentMapper(
    ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<BannerlordEquipmentMapper>();

    public Equipment MapToDomain(
        TaleWorlds.Core.Equipment bannerlordEquipment)
    {
        if (bannerlordEquipment is null)
        {
            _logger.Error("Bannerlord equipment is null");
            return new Equipment(new List<EquipmentSlot>());
        }

        var equipmentSlots = new List<EquipmentSlot>();

        for (int index = 0; index < (int)EquipmentIndex.NumEquipmentSetSlots; index++)
        {
            EquipmentElement equipmentElement;
            try
            {
                if (bannerlordEquipment[index].IsEmpty) continue;

                equipmentElement = bannerlordEquipment[index];
            }
            catch (Exception)
            {
                continue;
            }

            string slotId = MapEquipmentSlot(index);
            var itemId = equipmentElement.Item?.StringId ?? "";
            equipmentSlots.Add(new EquipmentSlot(slotId, itemId));
        }

        return new Equipment(equipmentSlots);
    }

    private string MapEquipmentSlot(int index)
    {
        return index switch
        {
            0 => "Weapon0",
            1 => "Weapon1",
            2 => "Weapon2",
            3 => "Weapon3",
            4 => "ExtraWeaponSlot",
            5 => "Head",
            6 => "Body",
            7 => "Leg",
            8 => "Gloves",
            9 => "Cape",
            10 => "Horse",
            11 => "HorseHarness",
            _ => throw new ArgumentException($"Invalid equipment index {index}")
        };
    }
}
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.Mappers;

public class EquipmentMapper(
    MBObjectManager mbObjectManager,
    ILoggerFactory loggerFactory,
    EquipmentFactory equipmentFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<EquipmentMapper>();

    public Equipment Map(Domain.EquipmentPool.Model.Equipment equipment, MBEquipmentRoster bannerlordEquipmentPool)
    {
        var equipmentLoadout = MapEquipment(equipment);

        var nativeEquipmentLoadout =
            FindMatchingDomainEquipmentInBannerlordEquipmentPool(bannerlordEquipmentPool, equipmentLoadout);

        if (nativeEquipmentLoadout is null)
        {
            _logger.Error(
                $"Could not find an exact equipment roster with '{DisplayNonEmptySlotAndItemIds(equipmentLoadout)}' among " +
                $"'{bannerlordEquipmentPool.StringId}' equipment rosters. Using given equipment roster.");
            return equipmentLoadout;
        }

        return nativeEquipmentLoadout;
    }

    private Equipment? FindMatchingDomainEquipmentInBannerlordEquipmentPool(MBEquipmentRoster bannerlordEquipmentPool,
        Equipment equipment)
    {
        if (bannerlordEquipmentPool is null) return null;

        return bannerlordEquipmentPool.AllEquipments.Find(nativeEquipmentLoadout =>
        {
            for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot;
                 index < EquipmentIndex.NumEquipmentSetSlots;
                 index++)
                if (nativeEquipmentLoadout[index].Item != equipment[index].Item)
                    return false;

            return true;
        });
    }

    private Equipment MapEquipment(Domain.EquipmentPool.Model.Equipment equipment)
    {
        return equipment.GetEquipmentSlots()
            .Aggregate(equipmentFactory.CreateEquipment(Equipment.EquipmentType.Battle), (equipment1, slot) =>
            {
                try
                {
                    EquipmentIndex index = Equipment.GetEquipmentIndexFromOldEquipmentIndexName(slot.SlotId);
                    ItemObject? item = mbObjectManager.GetObject<ItemObject>(ParseItemId(slot.ItemId));
                    if (item is null)
                        _logger.Error($"Could not find an item with id '{slot.ItemId}'");
                    else
                        equipment1[index] = new EquipmentElement(item);
                }
                catch (ArgumentException e)
                {
                    _logger.Error($"Could not parse '{slot.SlotId}' as an EquipmentIndex. Error: {e.Message}");
                }

                return equipment1;
            });
    }

    private static string ParseItemId(string id)
    {
        string pattern = @"^(Item\.)?(.*)$";

        Match match = Regex.Match(id, pattern);

        return match.Success ? match.Groups[2].Value : id;
    }

    private string DisplayNonEmptySlotAndItemIds(Equipment equipmentLoadout)
    {
        var ids = Enumerable.Range((int)EquipmentIndex.WeaponItemBeginSlot, (int)EquipmentIndex.NumEquipmentSetSlots)
            .Select(i =>
            {
                var equipmentSlot = (EquipmentIndex)i;
                var equipment = equipmentLoadout.GetEquipmentFromSlot(equipmentSlot);
                if (equipment.IsEmpty) return string.Empty;
                return $"{Enum.GetName(typeof(EquipmentIndex), equipmentSlot)}: {equipment.Item.StringId}";
            })
            .Where(slotAndItemDisplay => !slotAndItemDisplay.IsEmpty());
        return string.Join(", ", ids);
    }
}
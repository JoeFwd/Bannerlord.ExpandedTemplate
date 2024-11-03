using System;
using System.Linq;
using System.Text.RegularExpressions;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.EquipmentPools.Mappers;

public class EquipmentMapper(MBObjectManager mbObjectManager, ILoggerFactory loggerFactory)
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
                $"Could not find {equipmentLoadout} among native '{bannerlordEquipmentPool.StringId}' equipment roster");
            return null;
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
                 index < EquipmentIndex.NumAllWeaponSlots;
                 index++)
                if (nativeEquipmentLoadout[index].Item != equipment[index].Item)
                    return false;

            return true;
        });
    }

    private Equipment MapEquipment(Domain.EquipmentPool.Model.Equipment equipment)
    {
        return equipment.GetEquipmentSlots()
            .Aggregate(new Equipment(false), (equipment1, slot) =>
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
                    _logger.Error($"Could not parse '{slot.SlotId}' as an EquipmentIndex");
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
}
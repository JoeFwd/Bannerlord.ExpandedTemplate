using System;
using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Battle;

public class BattleEquipmentRosterFilter
{
    public IDictionary<string, IList<EquipmentRoster>> Filter(
        IDictionary<string, IList<EquipmentRoster>> equipmentRostersByCharacter,
        IDictionary<string, IList<EquipmentRoster>> civilianEquipmentRostersByCharacter,
        IDictionary<string, IList<EquipmentRoster>> siegeEquipmentRostersByCharacter,
        IDictionary<string, IList<EquipmentRoster>> stealthEquipmentRostersByCharacter)
    {
        return equipmentRostersByCharacter.ToDictionary(character => character.Key,
            character => character.Value.Where(equipmentRoster =>
                    IsBattleEquipmentRoster(character.Key, equipmentRoster, civilianEquipmentRostersByCharacter,
                        siegeEquipmentRostersByCharacter, stealthEquipmentRostersByCharacter))
                .ToList() as IList<EquipmentRoster>);
    }

    private static bool IsBattleEquipmentRoster(string characterId, EquipmentRoster equipmentRoster,
        IDictionary<string, IList<EquipmentRoster>> civilianEquipmentRostersByCharacter,
        IDictionary<string, IList<EquipmentRoster>> siegeEquipmentRostersByCharacter,
        IDictionary<string, IList<EquipmentRoster>> stealthEquipmentRostersByCharacter)
    {
        if (bool.TryParse(equipmentRoster.ResolvedIsBattle, out bool isBattle) && isBattle)
            return true;

        return !Contains(characterId, equipmentRoster, civilianEquipmentRostersByCharacter) &&
               !Contains(characterId, equipmentRoster, siegeEquipmentRostersByCharacter) &&
               !Contains(characterId, equipmentRoster, stealthEquipmentRostersByCharacter);
    }

    private static bool Contains(string characterId, EquipmentRoster equipmentRoster,
        IDictionary<string, IList<EquipmentRoster>> equipmentRostersByCharacter)
    {
        return equipmentRostersByCharacter.TryGetValue(characterId, out IList<EquipmentRoster>? equipmentRosters) &&
               equipmentRosters.Contains(equipmentRoster);
    }
}

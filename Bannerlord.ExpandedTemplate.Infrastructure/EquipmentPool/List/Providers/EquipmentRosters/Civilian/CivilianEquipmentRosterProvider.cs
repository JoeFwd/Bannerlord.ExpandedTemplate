using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Civilian;

public class CivilianEquipmentRosterProvider : IEquipmentRostersProvider
{
    private readonly INpcCharacterWithResolvedEquipmentProvider _npcCharacterWithResolvedEquipmentProvider;

    public CivilianEquipmentRosterProvider(
        INpcCharacterWithResolvedEquipmentProvider npcCharacterWithResolvedEquipmentProvider)
    {
        _npcCharacterWithResolvedEquipmentProvider = npcCharacterWithResolvedEquipmentProvider;
    }

    public IDictionary<string, IList<EquipmentRoster>> GetEquipmentRostersByCharacter()
    {
        return _npcCharacterWithResolvedEquipmentProvider.GetNpcCharactersWithResolvedEquipmentRoster()
            .ToDictionary(character => character.Key, character => character.Value.Where(
                equipmentRoster =>
                {
                    bool.TryParse(equipmentRoster.IsCivilian, out bool isCivilian);
                    return isCivilian;
                }).ToList() as IList<EquipmentRoster>);
    }
}
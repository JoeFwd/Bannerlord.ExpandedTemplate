using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Siege;

public class SiegeEquipmentRosterProvider : IEquipmentRostersProvider
{
    private readonly INpcCharacterWithResolvedEquipmentProvider _npcCharacterWithResolvedEquipmentProvider;

    public SiegeEquipmentRosterProvider(
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
                    bool.TryParse(equipmentRoster.IsSiege, out bool isSiege);
                    return isSiege;
                }).ToList() as IList<EquipmentRoster>);
    }
}
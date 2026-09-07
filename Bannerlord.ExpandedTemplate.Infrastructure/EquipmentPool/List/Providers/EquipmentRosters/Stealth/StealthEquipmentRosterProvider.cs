using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Stealth;

public class StealthEquipmentRosterProvider : IEquipmentRostersProvider
{
    private readonly INpcCharacterWithResolvedEquipmentProvider _npcCharacterWithResolvedEquipmentProvider;

    public StealthEquipmentRosterProvider(
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
                    bool.TryParse(equipmentRoster.ResolvedIsStealth, out bool isStealth);
                    return isStealth;
                }).ToList() as IList<EquipmentRoster>);
    }
}

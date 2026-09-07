using System;
using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters;

public class FlaggedEquipmentRosterProvider : IEquipmentRostersProvider
{
    private readonly Func<EquipmentRoster, string?> _flag;
    private readonly INpcCharacterWithResolvedEquipmentProvider _npcCharacterWithResolvedEquipmentProvider;

    public FlaggedEquipmentRosterProvider(INpcCharacterWithResolvedEquipmentProvider npcCharacterWithResolvedEquipmentProvider,
        Func<EquipmentRoster, string?> flag)
    {
        _npcCharacterWithResolvedEquipmentProvider = npcCharacterWithResolvedEquipmentProvider;
        _flag = flag;
    }

    public IDictionary<string, IList<EquipmentRoster>> GetEquipmentRostersByCharacter()
    {
        return _npcCharacterWithResolvedEquipmentProvider.GetNpcCharactersWithResolvedEquipmentRoster()
            .ToDictionary(character => character.Key,
                character => character.Value.Where(equipmentRoster =>
                        bool.TryParse(_flag(equipmentRoster), out bool selected) && selected)
                    .ToList() as IList<EquipmentRoster>);
    }
}

using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Pool;

public class PoolEquipmentRosterProvider : IPoolEquipmentRosterProvider
{
    private readonly INpcCharacterWithResolvedEquipmentProvider _npcCharacterWithResolvedEquipmentProvider;

    public PoolEquipmentRosterProvider(
        INpcCharacterWithResolvedEquipmentProvider npcCharacterWithResolvedEquipmentProvider)
    {
        _npcCharacterWithResolvedEquipmentProvider = npcCharacterWithResolvedEquipmentProvider;
    }

    public IDictionary<string, IDictionary<string, IList<EquipmentRoster>>>
        GetEquipmentRostersByPoolAndCharacter()
    {
        return _npcCharacterWithResolvedEquipmentProvider.GetNpcCharactersWithResolvedEquipmentRoster()
            .ToDictionary(character => character.Key, character =>
                character.Value
                        .GroupBy(equipmentRoster => NormalisePool(equipmentRoster.Pool))
                        .ToDictionary(group => group.Key, group => group.ToList() as IList<EquipmentRoster>) as
                    IDictionary<string, IList<EquipmentRoster>>);
    }

    private static string NormalisePool(string? pool)
    {
        return int.TryParse(pool, out int poolId) ? poolId.ToString() : "0";
    }
}
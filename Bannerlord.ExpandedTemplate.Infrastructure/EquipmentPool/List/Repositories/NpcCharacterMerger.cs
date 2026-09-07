using System.Linq;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories;

public static class NpcCharacterMerger
{
    public static NpcCharacters MergeDuplicates(NpcCharacters npcCharacters)
    {
        return new NpcCharacters
        {
            NpcCharacter = npcCharacters.NpcCharacter.GroupBy(character => character.Id)
                .Select(MergeGroup)
                .ToList()
        };
    }

    private static NpcCharacter MergeGroup(IGrouping<string?, NpcCharacter> characters)
    {
        NpcCharacter firstCharacter = characters.First();
        return new NpcCharacter
        {
            Id = firstCharacter.Id,
            Equipments = new Equipments
            {
                EquipmentRoster = characters.SelectMany(character => character.Equipments.EquipmentRoster).ToList(),
                EquipmentSet = characters.SelectMany(character => character.Equipments.EquipmentSet).ToList()
            }
        };
    }
}

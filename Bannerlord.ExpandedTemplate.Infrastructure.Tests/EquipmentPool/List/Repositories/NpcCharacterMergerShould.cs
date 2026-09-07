using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Repositories;

public class NpcCharacterMergerShould
{
    [Test]
    public void CombineEquipmentFromCharactersWithTheSameId()
    {
        NpcCharacters characters = new()
        {
            NpcCharacter = new List<NpcCharacter>
            {
                Character("character", "helmet", "set-one"),
                Character("character", "boots", "set-two"),
                Character("other", "sword", "set-three")
            }
        };

        NpcCharacters merged = NpcCharacterMerger.MergeDuplicates(characters);

        Assert.That(merged.NpcCharacter, Has.Count.EqualTo(2));
        NpcCharacter character = merged.NpcCharacter.Single(npcCharacter => npcCharacter.Id == "character");
        Assert.That(character.Equipments.EquipmentRoster.Select(roster => roster.Equipment[0].Id),
            Is.EqualTo(new[] { "helmet", "boots" }));
        Assert.That(character.Equipments.EquipmentSet.Select(set => set.Id), Is.EqualTo(new[] { "set-one", "set-two" }));
    }

    private static NpcCharacter Character(string id, string itemId, string setId)
    {
        return new NpcCharacter
        {
            Id = id,
            Equipments = new Equipments
            {
                EquipmentRoster = new List<EquipmentRoster>
                {
                    new() { Equipment = new List<Equipment> { new() { Id = itemId } } }
                },
                EquipmentSet = new List<EquipmentSet> { new() { Id = setId } }
            }
        };
    }
}

using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Providers.EquipmentRosters;

public class FlaggedEquipmentRosterProviderShould
{
    [Test]
    public void ReturnOnlyRostersSelectedByTheFlag()
    {
        var source = new Mock<INpcCharacterWithResolvedEquipmentProvider>();
        EquipmentRoster civilianRoster = new() { IsCivilian = "true" };
        EquipmentRoster otherRoster = new();
        source.Setup(provider => provider.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                ["character"] = new List<EquipmentRoster> { civilianRoster, otherRoster }
            });
        var provider = new FlaggedEquipmentRosterProvider(source.Object, roster => roster.ResolvedIsCivilian);

        IDictionary<string, IList<EquipmentRoster>> rosters = provider.GetEquipmentRostersByCharacter();

        Assert.That(rosters["character"], Is.EqualTo(new List<EquipmentRoster> { civilianRoster }));
    }

    [TestCase("Civilian")]
    [TestCase("Siege")]
    [TestCase("Stealth")]
    public void SupportResolvedEquipmentTypes(string equipmentType)
    {
        var source = new Mock<INpcCharacterWithResolvedEquipmentProvider>();
        EquipmentRoster roster = new() { EquipmentType = equipmentType };
        source.Setup(provider => provider.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>> { ["character"] = new List<EquipmentRoster> { roster } });

        var provider = new FlaggedEquipmentRosterProvider(source.Object, equipmentType switch
        {
            "Civilian" => equipmentRoster => equipmentRoster.ResolvedIsCivilian,
            "Siege" => equipmentRoster => equipmentRoster.ResolvedIsSiege,
            _ => equipmentRoster => equipmentRoster.ResolvedIsStealth
        });

        Assert.That(provider.GetEquipmentRostersByCharacter()["character"], Is.EqualTo(new List<EquipmentRoster> { roster }));
    }
}

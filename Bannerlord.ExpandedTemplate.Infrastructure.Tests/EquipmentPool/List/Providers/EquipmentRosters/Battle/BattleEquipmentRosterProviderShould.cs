using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Battle;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Providers.EquipmentRosters.Battle;

public class BattleEquipmentRosterProviderShould
{
    [Test]
    public void CoordinateSourceAndTypeProvidersThroughTheBattleFilter()
    {
        var source = new Mock<INpcCharacterWithResolvedEquipmentProvider>();
        var civilian = new Mock<IEquipmentRostersProvider>();
        var siege = new Mock<IEquipmentRostersProvider>();
        var stealth = new Mock<IEquipmentRostersProvider>();
        EquipmentRoster battle = new() { IsBattle = "true" };
        EquipmentRoster civilianRoster = new() { IsCivilian = "true" };
        IDictionary<string, IList<EquipmentRoster>> all = new Dictionary<string, IList<EquipmentRoster>>
        {
            ["character"] = new List<EquipmentRoster> { battle, civilianRoster }
        };
        source.Setup(provider => provider.GetNpcCharactersWithResolvedEquipmentRoster()).Returns(all);
        civilian.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(new Dictionary<string, IList<EquipmentRoster>> { ["character"] = new List<EquipmentRoster> { civilianRoster } });
        siege.Setup(provider => provider.GetEquipmentRostersByCharacter()).Returns(new Dictionary<string, IList<EquipmentRoster>>());
        stealth.Setup(provider => provider.GetEquipmentRostersByCharacter()).Returns(new Dictionary<string, IList<EquipmentRoster>>());
        var provider = new BattleEquipmentRosterProvider(siege.Object, civilian.Object, stealth.Object, source.Object,
            new BattleEquipmentRosterFilter());

        IDictionary<string, IList<EquipmentRoster>> rosters = provider.GetEquipmentRostersByCharacter();

        Assert.That(rosters["character"], Is.EqualTo(new List<EquipmentRoster> { battle }));
    }
}

using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Battle;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Providers.EquipmentRosters.Battle;

public class BattleEquipmentRosterFilterShould
{
    [Test]
    public void KeepUntaggedAndExplicitBattleRosters()
    {
        EquipmentRoster untagged = new();
        EquipmentRoster battle = new() { IsBattle = "true" };

        IDictionary<string, IList<EquipmentRoster>> rosters = new BattleEquipmentRosterFilter().Filter(
            RosterDictionary(untagged, battle), EmptyRosters(), EmptyRosters(), EmptyRosters());

        Assert.That(rosters["character"], Is.EqualTo(new List<EquipmentRoster> { untagged, battle }));
    }

    [TestCase("Civilian")]
    [TestCase("Siege")]
    [TestCase("Stealth")]
    public void ExcludeRostersClaimedByAnotherType(string equipmentType)
    {
        EquipmentRoster roster = new() { EquipmentType = equipmentType };
        IDictionary<string, IList<EquipmentRoster>> selected = RosterDictionary(roster);

        IDictionary<string, IList<EquipmentRoster>> rosters = new BattleEquipmentRosterFilter().Filter(selected,
            equipmentType == "Civilian" ? selected : EmptyRosters(),
            equipmentType == "Siege" ? selected : EmptyRosters(),
            equipmentType == "Stealth" ? selected : EmptyRosters());

        Assert.That(rosters["character"], Is.Empty);
    }

    private static IDictionary<string, IList<EquipmentRoster>> RosterDictionary(params EquipmentRoster[] rosters)
    {
        return new Dictionary<string, IList<EquipmentRoster>> { ["character"] = new List<EquipmentRoster>(rosters) };
    }

    private static IDictionary<string, IList<EquipmentRoster>> EmptyRosters()
    {
        return new Dictionary<string, IList<EquipmentRoster>>();
    }
}

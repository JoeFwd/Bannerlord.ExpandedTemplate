using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;
using NUnit.Framework;
using NpcEquipmentSet = Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters.EquipmentSet;
using RosterEquipmentSet =
    Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters.EquipmentSet;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Mappers;

public class EquipmentSetFlagMapperShould
{
    private readonly EquipmentSetFlagMapper _mapper = new();

    [Test]
    public void MatchesUntaggedLegacySets()
    {
        Assert.That(_mapper.IsMatching(new RosterEquipmentSet { IsCivilian = "legacy" }, new NpcEquipmentSet()), Is.True);
    }

    [Test]
    public void MatchesEachSharedLegacyFlag()
    {
        var characterSet = new NpcEquipmentSet
        {
            IsBattle = "true",
            IsCivilian = "true",
            IsSiege = "true"
        };

        Assert.That(_mapper.IsMatching(new RosterEquipmentSet { IsBattle = "true" }, characterSet), Is.True);
        Assert.That(_mapper.IsMatching(new RosterEquipmentSet { IsCivilian = "true" }, characterSet), Is.True);
        Assert.That(_mapper.IsMatching(new RosterEquipmentSet { IsSiege = "true" }, characterSet), Is.True);
    }

    [Test]
    public void MatchesUntaggedLegacySetWhenCharacterUsesLegacyFlags()
    {
        Assert.That(_mapper.IsMatching(
            new RosterEquipmentSet(),
            new NpcEquipmentSet { IsBattle = "true", IsCivilian = "true", IsSiege = "true" }), Is.True);
    }

    [Test]
    public void MatchesStealthWhenBothSetsSelectStealth()
    {
        Assert.That(_mapper.IsMatching(
            new RosterEquipmentSet { IsStealth = "true" },
            new NpcEquipmentSet { EquipmentType = "Stealth" }), Is.True);
    }

    [Test]
    public void DoesNotMatchExplicitlyFalseBattleSetWhenCharacterUsesLegacyBattleFlag()
    {
        Assert.That(_mapper.IsMatching(
            new RosterEquipmentSet { IsBattle = "false" },
            new NpcEquipmentSet { IsBattle = "true" }), Is.False);
    }

    [Test]
    public void MatchesModernEquipmentTypeWithTheEquivalentLegacyFlag()
    {
        Assert.That(_mapper.IsMatching(
            new RosterEquipmentSet { IsCivilian = "true" },
            new NpcEquipmentSet { EquipmentType = "Civilian" }), Is.True);
    }

    [Test]
    public void DoesNotMatchModernEquipmentTypeWithADifferentLegacyFlag()
    {
        Assert.That(_mapper.IsMatching(
            new RosterEquipmentSet { IsBattle = "true" },
            new NpcEquipmentSet { EquipmentType = "Civilian" }), Is.False);
    }

    [Test]
    public void DoesNotMatchExplicitlyFalseBattleSetWhenCharacterUsesModernBattleType()
    {
        Assert.That(_mapper.IsMatching(
            new RosterEquipmentSet { IsBattle = "false" },
            new NpcEquipmentSet { EquipmentType = "Battle" }), Is.False);
    }

    [TestCase("Civilian")]
    [TestCase("Siege")]
    [TestCase("Stealth")]
    public void DoesNotMatchUntaggedLegacySetWhenCharacterUsesNonBattleEquipmentType(string equipmentType)
    {
        Assert.That(_mapper.IsMatching(
            new RosterEquipmentSet(),
            new NpcEquipmentSet { EquipmentType = equipmentType }), Is.False);
    }

    [TestCase("Battle", null)]
    [TestCase(null, "Battle;Stealth")]
    public void MatchesUntaggedLegacySetWhenCharacterSelectsBattle(string? equipmentType, string? equipmentTypes)
    {
        Assert.That(_mapper.IsMatching(
            new RosterEquipmentSet(),
            new NpcEquipmentSet { EquipmentType = equipmentType, EquipmentTypes = equipmentTypes }), Is.True);
    }

    [Test]
    public void MatchesAnyTypeInEquipmentTypes()
    {
        var characterSet = new NpcEquipmentSet { EquipmentTypes = "Battle;Stealth" };

        Assert.That(_mapper.IsMatching(new RosterEquipmentSet { IsStealth = "true" }, characterSet), Is.True);
        Assert.That(_mapper.IsMatching(new RosterEquipmentSet { IsSiege = "true" }, characterSet), Is.False);
    }
}

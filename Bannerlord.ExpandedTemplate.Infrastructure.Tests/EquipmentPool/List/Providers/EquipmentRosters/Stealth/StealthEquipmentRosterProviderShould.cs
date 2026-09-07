using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Stealth;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Providers.EquipmentRosters.Stealth;

public class StealthEquipmentRosterProviderShould
{
    private Mock<INpcCharacterWithResolvedEquipmentProvider> _npcCharacterWithResolvedEquipmentProvider;
    private IEquipmentRostersProvider _stealthEquipmentRosterProvider;

    [SetUp]
    public void SetUp()
    {
        _npcCharacterWithResolvedEquipmentProvider = new Mock<INpcCharacterWithResolvedEquipmentProvider>();
        _stealthEquipmentRosterProvider =
            new StealthEquipmentRosterProvider(_npcCharacterWithResolvedEquipmentProvider.Object);
    }

    [Test]
    public void GetStealthEquipmentRostersTaggedByEquipmentType()
    {
        EquipmentRoster stealthRoster = CreateEquipmentRoster("StealthEquipment") with { EquipmentType = "Stealth" };
        _npcCharacterWithResolvedEquipmentProvider.Setup(provider => provider.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                { "Character1", new List<EquipmentRoster> { CreateEquipmentRoster("BattleEquipment"), stealthRoster } }
            });

        var rostersByCharacter = _stealthEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        Assert.That(rostersByCharacter["Character1"], Is.EqualTo(new List<EquipmentRoster> { stealthRoster }));
    }

    [Test]
    public void GetStealthEquipmentRostersTaggedByEquipmentTypes()
    {
        EquipmentRoster sharedRoster = CreateEquipmentRoster("SharedEquipment") with
        {
            EquipmentTypes = "Battle; Stealth"
        };
        _npcCharacterWithResolvedEquipmentProvider.Setup(provider => provider.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                { "Character1", new List<EquipmentRoster> { sharedRoster } }
            });

        var rostersByCharacter = _stealthEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        Assert.That(rostersByCharacter["Character1"], Is.EqualTo(new List<EquipmentRoster> { sharedRoster }));
    }

    [Test]
    public void DoesNotReturnEquipmentRostersWithInvalidEquipmentTypes()
    {
        _npcCharacterWithResolvedEquipmentProvider.Setup(provider => provider.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment1") with { EquipmentTypes = "Battle;Invalid" },
                        CreateEquipmentRoster("Equipment2") with { EquipmentType = "invalid" }
                    }
                }
            });

        var rostersByCharacter = _stealthEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        Assert.That(rostersByCharacter["Character1"], Is.Empty);
    }

    private static EquipmentRoster CreateEquipmentRoster(params string[] equipmentIds)
    {
        return new EquipmentRoster
        {
            Equipment = equipmentIds.Select(id => new Equipment { Id = id }).ToList()
        };
    }
}

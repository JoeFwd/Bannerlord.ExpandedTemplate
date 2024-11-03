using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Siege;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Providers.EquipmentRosters.Siege;

public class SiegeEquipmentRosterProviderShould
{
    private Mock<INpcCharacterWithResolvedEquipmentProvider> _npcCharacterWithResolvedEquipmentProvider;
    private IEquipmentRostersProvider _siegeEquipmentRosterProvider;

    [SetUp]
    public void SetUp()
    {
        _npcCharacterWithResolvedEquipmentProvider = new Mock<INpcCharacterWithResolvedEquipmentProvider>();
        _siegeEquipmentRosterProvider =
            new SiegeEquipmentRosterProvider(_npcCharacterWithResolvedEquipmentProvider.Object);
    }

    [Test]
    public void GetSiegeEquipmentRosters()
    {
        _npcCharacterWithResolvedEquipmentProvider.Setup(repo => repo.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment1", "Equipment2"),
                        CreateEquipmentRoster("Equipment3", "Equipment4") with { IsSiege = "true" }
                    }
                },
                {
                    "Character2", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment5", "Equipment6"),
                        CreateEquipmentRoster("Equipment5", "Equipment6")
                    }
                },
                {
                    "Character3", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment5", "Equipment6") with { IsSiege = "true" },
                        CreateEquipmentRoster("Equipment5", "Equipment6") with { IsSiege = "true" }
                    }
                }
            });

        var civilianEquipmentRostersByCharacter =
            _siegeEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        Assert.That(civilianEquipmentRostersByCharacter, Is.EqualTo(
            new Dictionary<string, List<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment3", "Equipment4") with { IsSiege = "true" }
                    }
                },
                {
                    "Character2", new List<EquipmentRoster>()
                },
                {
                    "Character3", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment5", "Equipment6") with { IsSiege = "true" },
                        CreateEquipmentRoster("Equipment5", "Equipment6") with { IsSiege = "true" }
                    }
                }
            }));
    }

    [Test]
    public void NotReturnEquipmentPools_WhenInvalidSymbolsAreUsedInCondition()
    {
        _npcCharacterWithResolvedEquipmentProvider.Setup(repo => repo.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment3", "Equipment4") with { IsSiege = "" },
                        CreateEquipmentRoster("Equipment3", "Equipment4") with { IsSiege = "invalid_pool" },
                        CreateEquipmentRoster("Equipment3", "Equipment4") with { IsSiege = "&amp;" },
                        CreateEquipmentRoster("Equipment3", "Equipment4") with { IsSiege = "         " },
                        CreateEquipmentRoster("Equipment3", "Equipment4") with { IsSiege = "false" }
                    }
                }
            });

        var civilianEquipmentRostersByCharacter =
            _siegeEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        Assert.That(civilianEquipmentRostersByCharacter, Is.EqualTo(
            new Dictionary<string, List<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>()
                }
            }));
    }

    private EquipmentRoster CreateEquipmentRoster(params string[] equipmentIds)
    {
        return new EquipmentRoster
        {
            Equipment = equipmentIds.Select(id => new Equipment
            {
                Id = id
            }).ToList()
        };
    }
}
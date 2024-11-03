using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Civilian;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Providers.EquipmentRosters.Civilian;

public class CivilianEquipmentRosterProviderShould
{
    private Mock<INpcCharacterWithResolvedEquipmentProvider> _npcCharacterWithResolvedEquipmentProvider;
    private IEquipmentRostersProvider _civilianEquipmentRosterProvider;

    [SetUp]
    public void SetUp()
    {
        _npcCharacterWithResolvedEquipmentProvider = new Mock<INpcCharacterWithResolvedEquipmentProvider>();
        _civilianEquipmentRosterProvider =
            new CivilianEquipmentRosterProvider(_npcCharacterWithResolvedEquipmentProvider.Object);
    }

    [Test]
    public void GetCivilianEquipmentRosters()
    {
        _npcCharacterWithResolvedEquipmentProvider.Setup(repo => repo.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment1", "Equipment2"),
                        CreateEquipmentRoster("Equipment3", "Equipment4") with { IsCivilian = "true" }
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
                        CreateEquipmentRoster("Equipment5", "Equipment6") with { IsCivilian = "true" },
                        CreateEquipmentRoster("Equipment5", "Equipment6") with { IsCivilian = "true" }
                    }
                }
            });

        var civilianEquipmentRostersByCharacter =
            _civilianEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        Assert.That(civilianEquipmentRostersByCharacter, Is.EqualTo(
            new Dictionary<string, List<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment3", "Equipment4") with { IsCivilian = "true" }
                    }
                },
                {
                    "Character2", new List<EquipmentRoster>()
                },
                {
                    "Character3", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment5", "Equipment6") with { IsCivilian = "true" },
                        CreateEquipmentRoster("Equipment5", "Equipment6") with { IsCivilian = "true" }
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
                        CreateEquipmentRoster("Equipment3", "Equipment4") with { IsCivilian = "" },
                        CreateEquipmentRoster("Equipment3", "Equipment4") with { IsCivilian = "invalid_pool" },
                        CreateEquipmentRoster("Equipment3", "Equipment4") with { IsCivilian = "&amp;" },
                        CreateEquipmentRoster("Equipment3", "Equipment4") with { IsCivilian = "         " },
                        CreateEquipmentRoster("Equipment3", "Equipment4") with { IsCivilian = "false" }
                    }
                }
            });

        var civilianEquipmentRostersByCharacter =
            _civilianEquipmentRosterProvider.GetEquipmentRostersByCharacter();

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
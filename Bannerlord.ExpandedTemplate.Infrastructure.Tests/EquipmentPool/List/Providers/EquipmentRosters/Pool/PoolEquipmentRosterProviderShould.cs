using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Pool;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Providers.EquipmentRosters.Pool;

public class PoolEquipmentRosterProviderShould
{
    private Mock<INpcCharacterWithResolvedEquipmentProvider> _npcCharacterWithResolvedEquipmentProvider;
    private IPoolEquipmentRosterProvider _poolEquipmentRosterProvider;

    [SetUp]
    public void SetUp()
    {
        _npcCharacterWithResolvedEquipmentProvider = new Mock<INpcCharacterWithResolvedEquipmentProvider>();
        _poolEquipmentRosterProvider =
            new PoolEquipmentRosterProvider(_npcCharacterWithResolvedEquipmentProvider.Object);
    }

    [Test]
    public void GroupEquipmentRostersWithNonIntegerPoolsIntoDefaultPool()
    {
        _npcCharacterWithResolvedEquipmentProvider.Setup(repo => repo.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment1", "Equipment2") with { Pool = "" },
                        CreateEquipmentRoster("Equipment2", "Equipment3") with { Pool = "invalid_pool" },
                        CreateEquipmentRoster("Equipment4", "Equipment5") with { Pool = "&" },
                        CreateEquipmentRoster("Equipment6", "Equipment7") with { Pool = "         " },
                        CreateEquipmentRoster("Equipment8", "Equipment9")
                    }
                }
            });

        var equipmentRostersByCharacter = _poolEquipmentRosterProvider.GetEquipmentRostersByPoolAndCharacter();

        Assert.That(equipmentRostersByCharacter, Is.EqualTo(
            new Dictionary<string, IDictionary<string, IList<EquipmentRoster>>>
            {
                {
                    "Character1", new Dictionary<string, IList<EquipmentRoster>>
                    {
                        {
                            "0",
                            new List<EquipmentRoster>
                            {
                                CreateEquipmentRoster("Equipment1", "Equipment2") with { Pool = "" },
                                CreateEquipmentRoster("Equipment2", "Equipment3") with { Pool = "invalid_pool" },
                                CreateEquipmentRoster("Equipment4", "Equipment5") with { Pool = "&" },
                                CreateEquipmentRoster("Equipment6", "Equipment7") with { Pool = "         " },
                                CreateEquipmentRoster("Equipment8", "Equipment9")
                            }
                        }
                    }
                }
            }));
    }

    [Test]
    public void GroupEquipmentRostersByPoolAndCharacter()
    {
        _npcCharacterWithResolvedEquipmentProvider.Setup(repo => repo.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment1", "Equipment2") with { Pool = "0" },
                        CreateEquipmentRoster("Equipment3", "Equipment4") with { Pool = "0" }
                    }
                },
                {
                    "Character2", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment5", "Equipment6") with { Pool = "0" },
                        CreateEquipmentRoster("Equipment5", "Equipment6") with { Pool = "1" }
                    }
                },
                {
                    "Character3", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment1", "Equipment2") with { Pool = "2" },
                        CreateEquipmentRoster("Equipment3", "Equipment4"),
                        CreateEquipmentRoster("Equipment5", "Equipment6") with { Pool = "1" },
                        CreateEquipmentRoster("Equipment7", "Equipment7") with { Pool = "0" }
                    }
                }
            });

        var equipmentRostersByCharacter = _poolEquipmentRosterProvider.GetEquipmentRostersByPoolAndCharacter();

        Assert.That(equipmentRostersByCharacter, Is.EqualTo(
            new Dictionary<string, IDictionary<string, IList<EquipmentRoster>>>
            {
                {
                    "Character1", new Dictionary<string, IList<EquipmentRoster>>
                    {
                        {
                            "0",
                            new List<EquipmentRoster>
                            {
                                CreateEquipmentRoster("Equipment1", "Equipment2") with { Pool = "0" },
                                CreateEquipmentRoster("Equipment3", "Equipment4") with { Pool = "0" }
                            }
                        }
                    }
                },
                {
                    "Character2", new Dictionary<string, IList<EquipmentRoster>>
                    {
                        {
                            "0",
                            new List<EquipmentRoster>
                            {
                                CreateEquipmentRoster("Equipment5", "Equipment6") with { Pool = "0" }
                            }
                        },
                        {
                            "1",
                            new List<EquipmentRoster>
                            {
                                CreateEquipmentRoster("Equipment5", "Equipment6") with { Pool = "1" }
                            }
                        }
                    }
                },
                {
                    "Character3", new Dictionary<string, IList<EquipmentRoster>>
                    {
                        {
                            "0",
                            new List<EquipmentRoster>
                            {
                                CreateEquipmentRoster("Equipment3", "Equipment4"),
                                CreateEquipmentRoster("Equipment7", "Equipment7") with { Pool = "0" }
                            }
                        },
                        {
                            "1",
                            new List<EquipmentRoster>
                            {
                                CreateEquipmentRoster("Equipment5", "Equipment6") with { Pool = "1" }
                            }
                        },
                        {
                            "2",
                            new List<EquipmentRoster>
                            {
                                CreateEquipmentRoster("Equipment1", "Equipment2") with { Pool = "2" }
                            }
                        }
                    }
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
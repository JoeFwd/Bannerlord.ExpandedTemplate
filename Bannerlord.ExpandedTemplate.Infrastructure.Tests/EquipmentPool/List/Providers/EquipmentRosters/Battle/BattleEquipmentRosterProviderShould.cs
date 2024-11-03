using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Battle;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Providers.EquipmentRosters.Battle;

public class BattleEquipmentRosterProviderShould
{
    private const string CachedObjectId = "irrelevant_cached_object_id";

    private Mock<ICacheProvider> _cacheProvider;
    private Mock<IEquipmentRostersProvider> _civilianEquipmentRosterProvider;
    private Mock<IEquipmentRostersProvider> _siegeEquipmentRosterProvider;
    private Mock<INpcCharacterWithResolvedEquipmentProvider> _npcCharacterWithResolvedEquipmentProvider;
    private Mock<ILogger> _logger;
    private Mock<ILoggerFactory> _loggerFactory;
    private IEquipmentRostersProvider _battleEquipmentRosterProvider;

    [SetUp]
    public void SetUp()
    {
        _civilianEquipmentRosterProvider = new Mock<IEquipmentRostersProvider>();
        _siegeEquipmentRosterProvider = new Mock<IEquipmentRostersProvider>();
        _npcCharacterWithResolvedEquipmentProvider = new Mock<INpcCharacterWithResolvedEquipmentProvider>();
        _cacheProvider = new Mock<ICacheProvider>();
        _logger = new Mock<ILogger>();
        _loggerFactory = new Mock<ILoggerFactory>();
        _loggerFactory.Setup(factory => factory.CreateLogger<BattleEquipmentRosterProvider>())
            .Returns(_logger.Object);

        _battleEquipmentRosterProvider = new BattleEquipmentRosterProvider(_loggerFactory.Object, _cacheProvider.Object,
            _siegeEquipmentRosterProvider.Object, _civilianEquipmentRosterProvider.Object,
            _npcCharacterWithResolvedEquipmentProvider.Object);
    }

    [Test]
    public void GetEquipmentPools()
    {
        _npcCharacterWithResolvedEquipmentProvider.Setup(repo => repo.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment1", "Equipment2"),
                        CreateEquipmentRoster("Equipment3", "Equipment4")
                    }
                },
                {
                    "Character2", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment5", "Equipment6"),
                        CreateEquipmentRoster("Equipment3", "Equipment7")
                    }
                }
            });

        _civilianEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>());

        _siegeEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>());

        var equipmentRosterByCharacter = _battleEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        Assert.That(equipmentRosterByCharacter, Is.EqualTo(new Dictionary<string, IList<EquipmentRoster>>
        {
            {
                "Character1", new List<EquipmentRoster>
                {
                    CreateEquipmentRoster("Equipment1", "Equipment2"),
                    CreateEquipmentRoster("Equipment3", "Equipment4")
                }
            },
            {
                "Character2", new List<EquipmentRoster>
                {
                    CreateEquipmentRoster("Equipment5", "Equipment6"),
                    CreateEquipmentRoster("Equipment3", "Equipment7")
                }
            }
        }));
    }

    [Test]
    public void NotGetCivilianCharacterEquipment()
    {
        _npcCharacterWithResolvedEquipmentProvider.Setup(repo => repo.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("CivilianEquipment1", "CivilianEquipment2"),
                        CreateEquipmentRoster("CivilianEquipment3", "CivilianEquipment4")
                    }
                },
                {
                    "Character2", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("CivilianEquipment5", "CivilianEquipment6"),
                        CreateEquipmentRoster("Equipment3", "CivilianEquipment7"),
                        CreateEquipmentRoster("Equipment3", "CivilianEquipment7")
                    }
                },
                {
                    "Character3", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("CivilianEquipment5", "CivilianEquipment6")
                    }
                }
            });

        _civilianEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1",
                    new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("CivilianEquipment1", "CivilianEquipment2"),
                        CreateEquipmentRoster("CivilianEquipment3", "CivilianEquipment4")
                    }
                },
                {
                    "Character2",
                    new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("CivilianEquipment5", "CivilianEquipment6"),
                        CreateEquipmentRoster("CivilianEquipment7")
                    }
                }
            });

        _siegeEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>());

        var equipmentRosterByCharacter = _battleEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        Assert.That(equipmentRosterByCharacter, Is.EqualTo(
            new Dictionary<string, List<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>()
                },
                {
                    "Character2", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment3", "CivilianEquipment7"),
                        CreateEquipmentRoster("Equipment3", "CivilianEquipment7")
                    }
                },
                {
                    "Character3", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("CivilianEquipment5", "CivilianEquipment6")
                    }
                }
            }));
    }

    [Test]
    public void GetCivilianCharacterEquipmentTaggedForBattle()
    {
        _npcCharacterWithResolvedEquipmentProvider.Setup(repo => repo.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1",
                    new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("CivilianEquipment1") with { IsBattle = "true" }
                    }
                }
            });

        _civilianEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1",
                    new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("CivilianEquipment1") with { IsBattle = "true" }
                    }
                }
            });
        _siegeEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>());

        var equipmentRosterByCharacter = _battleEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        Assert.That(equipmentRosterByCharacter, Is.EqualTo(
            new Dictionary<string, List<EquipmentRoster>>
            {
                {
                    "Character1",
                    new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("CivilianEquipment1") with { IsBattle = "true" }
                    }
                }
            }));
    }

    [Test]
    public void GetSiegeCharacterEquipmentTaggedForBattle()
    {
        _npcCharacterWithResolvedEquipmentProvider.Setup(repo => repo.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1",
                    new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("SiegeEquipment1") with { IsBattle = "true" }
                    }
                }
            });

        _civilianEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>());
        _siegeEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1",
                    new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("SiegeEquipment1") with { IsBattle = "true" }
                    }
                }
            });

        var equipmentRosterByCharacter = _battleEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        Assert.That(equipmentRosterByCharacter, Is.EqualTo(
            new Dictionary<string, List<EquipmentRoster>>
            {
                {
                    "Character1",
                    new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("SiegeEquipment1") with { IsBattle = "true" }
                    }
                }
            }));
    }

    [Test]
    public void GetCharacterEquipmentTaggedForBattleOnce()
    {
        _npcCharacterWithResolvedEquipmentProvider.Setup(repo => repo.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1",
                    new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("BattleEquipment1") with { IsBattle = "true" }
                    }
                }
            });
        _civilianEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1",
                    new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("BattleEquipment1") with { IsBattle = "true" }
                    }
                }
            });
        _siegeEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1",
                    new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("BattleEquipment1") with { IsBattle = "true" }
                    }
                }
            });

        var equipmentRosterByCharacter = _battleEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        Assert.That(equipmentRosterByCharacter, Is.EqualTo(
            new Dictionary<string, List<EquipmentRoster>>
            {
                {
                    "Character1",
                    new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("BattleEquipment1") with { IsBattle = "true" }
                    }
                }
            }));
    }

    [Test]
    public void NotGetSiegeCharacterEquipment()
    {
        _npcCharacterWithResolvedEquipmentProvider.Setup(repo => repo.GetNpcCharactersWithResolvedEquipmentRoster())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("SiegeEquipment1", "SiegeEquipment2"),
                        CreateEquipmentRoster("SiegeEquipment3", "SiegeEquipment4")
                    }
                },
                {
                    "Character2", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("SiegeEquipment5", "SiegeEquipment6"),
                        CreateEquipmentRoster("Equipment3", "SiegeEquipment7"),
                        CreateEquipmentRoster("Equipment3", "SiegeEquipment7")
                    }
                },
                {
                    "Character3", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("SiegeEquipment5", "SiegeEquipment6")
                    }
                }
            });
        _civilianEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>());
        _siegeEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("SiegeEquipment1", "SiegeEquipment2"),
                        CreateEquipmentRoster("SiegeEquipment3", "SiegeEquipment4")
                    }
                },
                {
                    "Character2", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("SiegeEquipment5", "SiegeEquipment6"),
                        CreateEquipmentRoster("Equipment3", "SiegeEquipment7")
                    }
                },
                {
                    "Character3", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("Equipment3", "SiegeEquipment7")
                    }
                }
            });

        var equipmentRosterByCharacter = _battleEquipmentRosterProvider.GetEquipmentRostersByCharacter();


        Assert.That(equipmentRosterByCharacter, Is.EqualTo(
            new Dictionary<string, List<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>()
                },
                {
                    "Character2", new List<EquipmentRoster>()
                },
                {
                    "Character3", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("SiegeEquipment5", "SiegeEquipment6")
                    }
                }
            }));
    }

    [Test]
    public void GetCachedEquipmentPools()
    {
        var equipmentByCharacterId = new Dictionary<string, IList<EquipmentRoster>>();
        _npcCharacterWithResolvedEquipmentProvider
            .Setup(provider => provider.GetNpcCharactersWithResolvedEquipmentRoster()).Returns(equipmentByCharacterId);
        var civilianEquipmentRosters = new Dictionary<string, IList<EquipmentRoster>>();
        _civilianEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(civilianEquipmentRosters);
        var siegeEquipmentRosters = new Dictionary<string, IList<EquipmentRoster>>();
        _siegeEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(siegeEquipmentRosters);

        _cacheProvider.Setup(provider => provider.CacheObject(It.IsAny<object>()))
            .Returns(CachedObjectId);
        _cacheProvider.Setup(provider =>
            provider.InvalidateCache(CachedObjectId, CampaignEvent.OnAfterSessionLaunched));

        var equipmentRosterByCharacter = _battleEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        _cacheProvider.Setup(cacheProvider =>
                cacheProvider.GetCachedObject<IDictionary<string, IList<EquipmentRoster>>>(
                    CachedObjectId))
            .Returns(equipmentRosterByCharacter);

        var cachedCharacterBattleEquipment = _battleEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        Assert.That(cachedCharacterBattleEquipment, Is.EqualTo(equipmentRosterByCharacter));
        _cacheProvider.VerifyAll();
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
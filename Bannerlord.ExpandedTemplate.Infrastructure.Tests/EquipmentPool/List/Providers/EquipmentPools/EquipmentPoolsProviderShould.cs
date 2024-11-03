using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Pool;
using Moq;
using NUnit.Framework;
using Equipment = Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters.Equipment;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Providers.EquipmentPools;

public class EquipmentPoolsProviderShould
{
    private Mock<IEquipmentRostersProvider> _battleEquipmentRosterProvider;
    private Mock<IPoolEquipmentRosterProvider> _poolEquipmentRosterProvider;
    private Mock<IEquipmentRosterMapper> _equipmentRosterMapper;
    private Mock<ICacheProvider> _cacheProvider;
    private IEquipmentPoolsProvider _equipmentPoolsProvider;

    private const string CachedObjectId = "irrevelant cache id";
    
    [SetUp]
    public void SetUp()
    {
        _battleEquipmentRosterProvider = new Mock<IEquipmentRostersProvider>(MockBehavior.Strict);
        _poolEquipmentRosterProvider = new Mock<IPoolEquipmentRosterProvider>(MockBehavior.Strict);
        _equipmentRosterMapper = new Mock<IEquipmentRosterMapper>(MockBehavior.Strict);
        _cacheProvider = new Mock<ICacheProvider>(MockBehavior.Strict);

        _equipmentPoolsProvider = new EquipmentPoolsProvider(_battleEquipmentRosterProvider.Object,
            _poolEquipmentRosterProvider.Object, _equipmentRosterMapper.Object, _cacheProvider.Object);
    }

    [Test]
    public void GetEquipmentPools()
    {
        _battleEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("SiegeEquipment1", "SiegeEquipment2")
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
                    "Character3", new List<EquipmentRoster>()
                }
            });
        _poolEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByPoolAndCharacter()).Returns(
            new Dictionary<string, IDictionary<string, IList<EquipmentRoster>>>
            {
                {
                    "Character1", new Dictionary<string, IList<EquipmentRoster>>
                    {
                        {
                            "0",
                            new List<EquipmentRoster>
                            {
                                CreateEquipmentRoster("SiegeEquipment1", "SiegeEquipment2"),
                                CreateEquipmentRoster("SiegeEquipment3", "SiegeEquipment4")
                            }
                        }
                    }
                },
                {
                    "Character2", new Dictionary<string, IList<EquipmentRoster>>
                    {
                        {
                            "2",
                            new List<EquipmentRoster>
                            {
                                CreateEquipmentRoster("SiegeEquipment5", "SiegeEquipment6")
                            }
                        },
                        {
                            "1",
                            new List<EquipmentRoster>
                            {
                                CreateEquipmentRoster("Equipment3", "SiegeEquipment7")
                            }
                        },
                        {
                            "4",
                            new List<EquipmentRoster>
                            {
                                CreateEquipmentRoster("Equipment3", "SiegeEquipment7")
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
                                CreateEquipmentRoster("SiegeEquipment5", "SiegeEquipment6")
                            }
                        }
                    }
                }
            });

        var equipmentPool1 = CreateEquipmentPool(0, "SiegeEquipment1", "SiegeEquipment2");
        var equipmentPool2 = CreateEquipmentPool(0, "SiegeEquipment5", "SiegeEquipment6");
        var equipmentPool3 = CreateEquipmentPool(0, "Equipment3", "SiegeEquipment7");
        var equipmentPool4 = CreateEquipmentPool(0, "Equipment3", "SiegeEquipment7");

        _equipmentRosterMapper
            .Setup(mapper =>
                mapper.MapToEquipmentPool(CreateEquipmentRoster("SiegeEquipment1", "SiegeEquipment2")))
            .Returns(equipmentPool1);
        _equipmentRosterMapper
            .Setup(mapper => mapper.MapToEquipmentPool(CreateEquipmentRoster("SiegeEquipment5", "SiegeEquipment6")))
            .Returns(equipmentPool2);
        _equipmentRosterMapper
            .Setup(mapper => mapper.MapToEquipmentPool(CreateEquipmentRoster("Equipment3", "SiegeEquipment7")))
            .Returns(equipmentPool3);
        _equipmentRosterMapper
            .Setup(mapper => mapper.MapToEquipmentPool(CreateEquipmentRoster("Equipment3", "SiegeEquipment7")))
            .Returns(equipmentPool4);

        _cacheProvider.Setup(provider => provider.CacheObject(It.IsAny<object>())).Returns(CachedObjectId);

        var battleEquipmentPoolByCharacterId = _equipmentPoolsProvider.GetEquipmentPoolsByCharacterId();

        Assert.That(battleEquipmentPoolByCharacterId, Is.EqualTo(
            new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1",
                    new List<Domain.EquipmentPool.Model.EquipmentPool>
                        { CreateEquipmentPool(0, "SiegeEquipment1", "SiegeEquipment2") }
                },
                {
                    "Character2",
                    new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        CreateEquipmentPool(2, "SiegeEquipment5", "SiegeEquipment6"),
                        CreateEquipmentPool(1, "Equipment3", "SiegeEquipment7"),
                        CreateEquipmentPool(4, "Equipment3", "SiegeEquipment7")
                    }
                },
                {
                    "Character3", new List<Domain.EquipmentPool.Model.EquipmentPool>()
                }
            }));
    }

    [Test]
    public void GetCachedEquipmentPoolsIfAvailable()
    {
        var result = SetupMocks();

        _cacheProvider.Setup(provider => provider.CacheObject(result)).Returns(CachedObjectId);

        // expected result to be cached on first call
        _equipmentPoolsProvider.GetEquipmentPoolsByCharacterId();

        _cacheProvider.Setup(provider =>
                provider.GetCachedObject<IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>>(
                    CachedObjectId))
            .Returns(result);

        var battleEquipmentPoolByCharacterId = _equipmentPoolsProvider.GetEquipmentPoolsByCharacterId();

        Assert.That(battleEquipmentPoolByCharacterId, Is.EqualTo(result));
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

    private static Domain.EquipmentPool.Model.EquipmentPool CreateEquipmentPool(int pool, params string[] equipmentIds)
    {
        return new Domain.EquipmentPool.Model.EquipmentPool(new List<Domain.EquipmentPool.Model.Equipment>
        {
            new(equipmentIds.Select(id => new EquipmentSlot(id, id)).ToList())
        }, pool);
    }

    private IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>> SetupMocks()
    {
        _battleEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByCharacter())
            .Returns(new Dictionary<string, IList<EquipmentRoster>>
            {
                {
                    "Character1", new List<EquipmentRoster>
                    {
                        CreateEquipmentRoster("SiegeEquipment1", "SiegeEquipment2")
                    }
                }
            });
        _poolEquipmentRosterProvider.Setup(provider => provider.GetEquipmentRostersByPoolAndCharacter()).Returns(
            new Dictionary<string, IDictionary<string, IList<EquipmentRoster>>>
            {
                {
                    "Character1", new Dictionary<string, IList<EquipmentRoster>>
                    {
                        {
                            "0",
                            new List<EquipmentRoster>
                            {
                                CreateEquipmentRoster("SiegeEquipment1", "SiegeEquipment2")
                            }
                        }
                    }
                }
            });

        var equipmentPool1 = CreateEquipmentPool(0, "SiegeEquipment1", "SiegeEquipment2");

        _equipmentRosterMapper
            .Setup(mapper =>
                mapper.MapToEquipmentPool(CreateEquipmentRoster("SiegeEquipment1", "SiegeEquipment2")))
            .Returns(equipmentPool1);

        _cacheProvider.Setup(provider => provider.CacheObject(It.IsAny<object>()))
            .Returns(CachedObjectId);

        return new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
        {
            {
                "Character1",
                new List<Domain.EquipmentPool.Model.EquipmentPool>
                    { CreateEquipmentPool(0, "SiegeEquipment1", "SiegeEquipment2") }
            }
        };
    }
}
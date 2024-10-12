﻿using System.Xml.Linq;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.Battle;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.Civilian;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.Siege;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Providers.Battle;

public class BattleEquipmentPoolProviderShould
{
    private const string CachedObjectId = "irrelevant_cached_object_id";

    private Mock<ICacheProvider> _cacheProvider;
    private Mock<ILogger> _logger;
    private Mock<ILoggerFactory> _loggerFactory;

    [SetUp]
    public void SetUp()
    {
        _cacheProvider = new Mock<ICacheProvider>();
        _logger = new Mock<ILogger>();
        _loggerFactory = new Mock<ILoggerFactory>();
        _loggerFactory.Setup(factory => factory.CreateLogger<BattleEquipmentPoolProvider>())
            .Returns(_logger.Object);
    }

    [Test]
    public void GetEquipmentPools()
    {
        var equipmentRepository = EquipmentRepositoryReturns(
            new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        CreateEquipmentPool(new[] { "Equipment1", "Equipment2" }, 0),
                        CreateEquipmentPool(new[] { "Equipment3", "Equipment4" }, 1)
                    }
                },
                {
                    "Character2", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        CreateEquipmentPool(new[] { "Equipment5", "Equipment6" }, 0),
                        CreateEquipmentPool(new[] { "Equipment3", "Equipment7" }, 1)
                    }
                }
            });
        var siegeEquipmentRepository =
            SiegeEquipmentRepositoryReturns(new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>());
        var civilianEquipmentRepository =
            CivilianEquipmentRepositoryReturns(
                new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>());
        var battleEquipmentRepository = new BattleEquipmentPoolProvider(_loggerFactory.Object, _cacheProvider.Object,
            siegeEquipmentRepository,
            civilianEquipmentRepository, equipmentRepository);

        var characterBattleEquipment = battleEquipmentRepository.GetBattleEquipmentByCharacterAndPool();

        Assert.That(characterBattleEquipment, Is.EqualTo(
            new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        CreateEquipmentPool(new[] { "Equipment1", "Equipment2" }, 0),
                        CreateEquipmentPool(new[] { "Equipment3", "Equipment4" }, 1)
                    }
                },
                {
                    "Character2", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        CreateEquipmentPool(new[] { "Equipment5", "Equipment6" }, 0),
                        CreateEquipmentPool(new[] { "Equipment3", "Equipment7" }, 1)
                    }
                }
            }));
    }

    [Test]
    public void NotGetCivilianCharacterEquipment()
    {
        var equipmentRepository = EquipmentRepositoryReturns(
            new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        CreateEquipmentPool(new[] { "CivilianEquipment1", "CivilianEquipment2" }, 0),
                        CreateEquipmentPool(new[] { "CivilianEquipment3", "CivilianEquipment4" }, 1)
                    }
                },
                {
                    "Character2", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        CreateEquipmentPool(new[] { "CivilianEquipment5", "CivilianEquipment6" }, 0),
                        CreateEquipmentPool(new[] { "Equipment3", "CivilianEquipment7" }, 1),
                        CreateEquipmentPool(new[] { "Equipment3", "CivilianEquipment7" }, 2)
                    }
                }
            });
        var siegeEquipmentRepository =
            SiegeEquipmentRepositoryReturns(new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>());
        var civilianEquipmentRepository =
            CivilianEquipmentRepositoryReturns(new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        CreateEquipmentPool(new[] { "CivilianEquipment1", "CivilianEquipment2" }, 0),
                        CreateEquipmentPool(new[] { "CivilianEquipment3", "CivilianEquipment4" }, 1)
                    }
                },
                {
                    "Character2", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        CreateEquipmentPool(new[] { "CivilianEquipment5", "CivilianEquipment6" }, 0),
                        CreateEquipmentPool(new[] { "CivilianEquipment7" }, 1)
                    }
                }
            });
        var battleEquipmentRepository = new BattleEquipmentPoolProvider(_loggerFactory.Object, _cacheProvider.Object,
            siegeEquipmentRepository,
            civilianEquipmentRepository, equipmentRepository);

        var characterBattleEquipment = battleEquipmentRepository.GetBattleEquipmentByCharacterAndPool();

        Assert.That(characterBattleEquipment, Is.EqualTo(
            new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>()
                },
                {
                    "Character2", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        CreateEquipmentPool(new[] { "Equipment3" }, 1),
                        CreateEquipmentPool(new[] { "Equipment3", "CivilianEquipment7" }, 2)
                    }
                }
            }));
    }

    [Test]
    public void GetCivilianCharacterEquipmentTaggedForBattle()
    {
        var equipmentRepository = EquipmentRepositoryReturns(
            new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        new(new List<Equipment>
                        {
                            CreateEquipmentWithBattleTag("CivilianEquipment1")
                        }, 0)
                    }
                }
            });
        var siegeEquipmentRepository =
            SiegeEquipmentRepositoryReturns(new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>());
        var civilianEquipmentRepository =
            CivilianEquipmentRepositoryReturns(new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        new(new List<Equipment>
                        {
                            CreateEquipmentWithBattleTag("CivilianEquipment1")
                        }, 0)
                    }
                }
            });
        var battleEquipmentRepository = new BattleEquipmentPoolProvider(_loggerFactory.Object, _cacheProvider.Object,
            siegeEquipmentRepository,
            civilianEquipmentRepository, equipmentRepository);

        var characterBattleEquipment = battleEquipmentRepository.GetBattleEquipmentByCharacterAndPool();

        Assert.That(characterBattleEquipment, Is.EqualTo(
            new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        new(new List<Equipment>
                        {
                            CreateEquipmentWithBattleTag("CivilianEquipment1")
                        }, 0)
                    }
                }
            }));
    }

    [Test]
    public void NotGetSiegeCharacterEquipment()
    {
        var equipmentRepository = EquipmentRepositoryReturns(
            new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        new(new List<Equipment>
                        {
                            CreateEquipmentWithBattleTag("SiegeEquipment1")
                        }, 0)
                    }
                }
            });
        var siegeEquipmentRepository =
            SiegeEquipmentRepositoryReturns(new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        new(new List<Equipment>
                        {
                            CreateEquipmentWithBattleTag("SiegeEquipment1")
                        }, 0)
                    }
                }
            });
        var civilianEquipmentRepository =
            CivilianEquipmentRepositoryReturns(
                new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>());
        var battleEquipmentRepository = new BattleEquipmentPoolProvider(_loggerFactory.Object, _cacheProvider.Object,
            siegeEquipmentRepository,
            civilianEquipmentRepository, equipmentRepository);

        var characterBattleEquipment = battleEquipmentRepository.GetBattleEquipmentByCharacterAndPool();

        Assert.That(characterBattleEquipment, Is.EqualTo(
            new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        new(new List<Equipment>
                        {
                            CreateEquipmentWithBattleTag("SiegeEquipment1")
                        }, 0)
                    }
                }
            })
        );
    }

    [Test]
    public void GetCharacterEquipmentTaggedForBattleOnce()
    {
        var equipmentRepository = EquipmentRepositoryReturns(
            new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        new(new List<Equipment>
                        {
                            CreateEquipmentWithBattleTag("BattleEquipment1")
                        }, 0)
                    }
                }
            });
        var siegeEquipmentRepository =
            SiegeEquipmentRepositoryReturns(new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        new(new List<Equipment>
                        {
                            CreateEquipmentWithBattleTag("BattleEquipment1")
                        }, 0)
                    }
                }
            });
        var civilianEquipmentRepository =
            CivilianEquipmentRepositoryReturns(new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        new(new List<Equipment>
                        {
                            CreateEquipmentWithBattleTag("BattleEquipment1")
                        }, 0)
                    }
                }
            });
        var battleEquipmentRepository = new BattleEquipmentPoolProvider(_loggerFactory.Object, _cacheProvider.Object,
            siegeEquipmentRepository,
            civilianEquipmentRepository, equipmentRepository);

        var characterBattleEquipment = battleEquipmentRepository.GetBattleEquipmentByCharacterAndPool();

        Assert.That(characterBattleEquipment, Is.EqualTo(
            new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        new(new List<Equipment>
                        {
                            CreateEquipmentWithBattleTag("BattleEquipment1")
                        }, 0)
                    }
                }
            })
        );
    }

    [Test]
    public void GetSiegeCharacterEquipmentTaggedForBattle()
    {
        var equipmentRepository = EquipmentRepositoryReturns(
            new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        CreateEquipmentPool(new[] { "SiegeEquipment1", "SiegeEquipment2" }, 0),
                        CreateEquipmentPool(new[] { "SiegeEquipment3", "SiegeEquipment4" }, 1)
                    }
                },
                {
                    "Character2", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        CreateEquipmentPool(new[] { "SiegeEquipment5", "SiegeEquipment6" }, 0),
                        CreateEquipmentPool(new[] { "Equipment3", "SiegeEquipment7" }, 1),
                        CreateEquipmentPool(new[] { "Equipment3", "SiegeEquipment7" }, 2)
                    }
                }
            });
        var siegeEquipmentRepository =
            SiegeEquipmentRepositoryReturns(new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        CreateEquipmentPool(new[] { "SiegeEquipment1", "SiegeEquipment2" }, 0),
                        CreateEquipmentPool(new[] { "SiegeEquipment3", "SiegeEquipment4" }, 1)
                    }
                },
                {
                    "Character2", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        CreateEquipmentPool(new[] { "SiegeEquipment5", "SiegeEquipment6" }, 0),
                        CreateEquipmentPool(new[] { "SiegeEquipment7" }, 1)
                    }
                }
            });
        var civilianEquipmentRepository =
            CivilianEquipmentRepositoryReturns(
                new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>());
        var battleEquipmentRepository = new BattleEquipmentPoolProvider(_loggerFactory.Object, _cacheProvider.Object,
            siegeEquipmentRepository,
            civilianEquipmentRepository, equipmentRepository);

        var characterBattleEquipment = battleEquipmentRepository.GetBattleEquipmentByCharacterAndPool();

        Assert.That(characterBattleEquipment, Is.EqualTo(
            new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    "Character1", new List<Domain.EquipmentPool.Model.EquipmentPool>()
                },
                {
                    "Character2", new List<Domain.EquipmentPool.Model.EquipmentPool>
                    {
                        CreateEquipmentPool(new[] { "Equipment3" }, 1),
                        CreateEquipmentPool(new[] { "Equipment3", "SiegeEquipment7" }, 2)
                    }
                }
            })
        );
    }

    [Test]
    public void GetCachedEquipmentPools()
    {
        var equipmentPools = new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>();
        var equipmentRepository = EquipmentRepositoryReturns(equipmentPools);
        var siegeEquipmentPools = new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>();
        var siegeEquipmentRepository = SiegeEquipmentRepositoryReturns(siegeEquipmentPools);
        var civilianEquipmentPools = new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>();
        var civilianEquipmentRepository = CivilianEquipmentRepositoryReturns(civilianEquipmentPools);
        var battleEquipmentRepository = new BattleEquipmentPoolProvider(_loggerFactory.Object, _cacheProvider.Object,
            siegeEquipmentRepository,
            civilianEquipmentRepository, equipmentRepository);

        _cacheProvider.Setup(provider => provider.CacheObject(It.IsAny<object>()))
            .Returns(CachedObjectId);
        _cacheProvider.Setup(provider =>
            provider.InvalidateCache(CachedObjectId, CampaignEvent.OnAfterSessionLaunched));

        var characterBattleEquipment = battleEquipmentRepository.GetBattleEquipmentByCharacterAndPool();

        _cacheProvider.Setup(cacheProvider =>
                cacheProvider.GetCachedObject<IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>>(
                    CachedObjectId))
            .Returns(characterBattleEquipment);

        var cachedCharacterBattleEquipment = battleEquipmentRepository.GetBattleEquipmentByCharacterAndPool();

        Assert.That(cachedCharacterBattleEquipment, Is.EqualTo(characterBattleEquipment));
        _cacheProvider.VerifyAll();
    }

    private IEquipmentPoolsRepository EquipmentRepositoryReturns(
        IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>> equipmentPoolsByCharacter)
    {
        var equipmentRepository = new Mock<IEquipmentPoolsRepository>();
        equipmentRepository.Setup(repo => repo.GetEquipmentPoolsById()).Returns(equipmentPoolsByCharacter);
        return equipmentRepository.Object;
    }

    private ICivilianEquipmentPoolProvider CivilianEquipmentRepositoryReturns(
        IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>> equipmentPoolsByCharacter)
    {
        var equipmentRepository = new Mock<ICivilianEquipmentPoolProvider>();
        equipmentRepository.Setup(repo => repo.GetCivilianEquipmentByCharacterAndPool())
            .Returns(equipmentPoolsByCharacter);
        return equipmentRepository.Object;
    }

    private ISiegeEquipmentPoolProvider SiegeEquipmentRepositoryReturns(
        IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>> equipmentPoolsByCharacter)
    {
        var equipmentRepository = new Mock<ISiegeEquipmentPoolProvider>();
        equipmentRepository.Setup(repo => repo.GetSiegeEquipmentByCharacterAndPool())
            .Returns(equipmentPoolsByCharacter);
        return equipmentRepository.Object;
    }

    private Domain.EquipmentPool.Model.EquipmentPool CreateEquipmentPool(string[] equipmentIds, int poolId)
    {
        var equipment = equipmentIds.Select(equipmentId => CreateUniqueEquipment(equipmentId)).ToList();
        return new Domain.EquipmentPool.Model.EquipmentPool(equipment, poolId);
    }

    private Equipment CreateUniqueEquipment(string id)
    {
        var xml = $"<EquipmentRoster id=\"{id}\">\n" +
                  "<equipment slot=\"Item0\" id=\"Item.ddg_polearm_longspear2\"/>\n" +
                  "<equipment slot=\"Body\" id=\"Item.jack_sleeveless_with_splints2\"/>\n" +
                  "<equipment slot=\"Leg\" id=\"Item.hosen_with_boots_c2\"/>\n" +
                  "<equipment slot=\"Head\" id=\"Item.war_hat2\"/>\n" +
                  "</EquipmentRoster>";

        return CreateEquipment(xml);
    }

    private Equipment CreateEquipmentWithBattleTag(string id)
    {
        var xml = $"<EquipmentRoster id=\"{id}\" battle=\"true\">\n" +
                  "<equipment slot=\"Item0\" id=\"Item.ddg_polearm_longspear2\"/>\n" +
                  "<equipment slot=\"Body\" id=\"Item.jack_sleeveless_with_splints2\"/>\n" +
                  "<equipment slot=\"Leg\" id=\"Item.hosen_with_boots_c2\"/>\n" +
                  "<equipment slot=\"Head\" id=\"Item.war_hat2\"/>\n" +
                  "</EquipmentRoster>";

        return CreateEquipment(xml);
    }

    private Equipment CreateEquipment(string xml)
    {
        return new Equipment(XDocument.Parse(xml).Root!);
    }
}
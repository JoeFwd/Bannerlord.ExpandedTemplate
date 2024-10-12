﻿using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.Civilian;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.Get;

public class TroopCivilianEquipmentProviderShould
{
    private const string firstTroopId = "first unrelevant troop id";
    private const string secondTroopId = "second unrelevant troop id";
    private const string cachedOnLoadEquipmentPoolsKey = "cachedOnLoadEquipmentPoolsKey";

    private Mock<IList<Domain.EquipmentPool.Model.EquipmentPool>> _troopEquipmentPools;

    private Mock<ILoggerFactory> _loggerFactory;
    private Mock<ICivilianEquipmentPoolProvider> _civilianEquipmentRepository;
    private Mock<ICacheProvider> _cacheProvider;
    private ITroopCivilianEquipmentProvider _troopCivilianEquipmentProvider;

    [SetUp]
    public void Setup()
    {
        _troopEquipmentPools = new Mock<IList<Domain.EquipmentPool.Model.EquipmentPool>>();
        _civilianEquipmentRepository = new Mock<ICivilianEquipmentPoolProvider>();
        _loggerFactory = new Mock<ILoggerFactory>();
        _loggerFactory.Setup(factory => factory.CreateLogger<TroopCivilianEquipmentProvider>())
            .Returns(new Mock<ILogger>().Object);
        _cacheProvider = new Mock<ICacheProvider>();
        _cacheProvider
            .Setup(
                cache => cache.CacheObject(It.IsAny<Func<object>>(), CampaignEvent.OnSessionLaunched))
            .Returns(cachedOnLoadEquipmentPoolsKey);

        _troopCivilianEquipmentProvider =
            new TroopCivilianEquipmentProvider(_loggerFactory.Object, _civilianEquipmentRepository.Object,
                _cacheProvider.Object);
    }


    [Test]
    public void ReturnNoEquipmentPoolsIfTroopIdIsEmpty()
    {
        var actualEquipmentPools = _troopCivilianEquipmentProvider.GetCivilianTroopEquipmentPools("");

        Assert.That(actualEquipmentPools, Is.Empty);
    }

    [Test]
    public void ReturnNoEquipmentPoolsIfTroopIdIsNull()
    {
        var actualEquipmentPools = _troopCivilianEquipmentProvider.GetCivilianTroopEquipmentPools(null!);

        Assert.That(actualEquipmentPools, Is.Empty);
    }

    [Test]
    public void ReturnCivilianEquipmentPools()
    {
        _cacheProvider
            .Setup(cache =>
                cache.GetCachedObject<IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>>(cachedOnLoadEquipmentPoolsKey))
            .Returns(new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                {
                    firstTroopId, _troopEquipmentPools.Object
                },
                {
                    secondTroopId, null!
                }
            });

        var actualEquipmentPools = _troopCivilianEquipmentProvider.GetCivilianTroopEquipmentPools(firstTroopId);

        Assert.That(actualEquipmentPools, Is.EqualTo(_troopEquipmentPools.Object));
    }

    [Test]
    public void ReturnNoEquipmentPoolsIfTroopIdDoesNotExist()
    {
        _cacheProvider
            .Setup(cache =>
                cache.GetCachedObject<IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>>(cachedOnLoadEquipmentPoolsKey))
            .Returns(new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>());

        var actualEquipmentPools = _troopCivilianEquipmentProvider.GetCivilianTroopEquipmentPools(firstTroopId);

        Assert.That(actualEquipmentPools, Is.Empty);
    }
}
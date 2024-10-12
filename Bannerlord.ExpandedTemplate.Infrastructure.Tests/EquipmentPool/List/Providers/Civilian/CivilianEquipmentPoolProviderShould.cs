using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.Civilian;
using Moq;
using NUnit.Framework;
using static Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.TestUtil.TestFolderComparator;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Providers.Civilian;

public class CivilianEquipmentPoolProviderShould
{
    private const string CachedObjectId = "irrelevant_cached_object_id";

    private readonly string _validSiegeEquipmentDataFolderPath =
        Path.Combine("Data", "CivilianEquipmentPoolProvider", "ValidSymbols");

    private readonly string _invalidSiegeEquipmentDataFolderPath =
        Path.Combine("Data", "CivilianEquipmentPoolProvider", "InvalidSymbols");

    private readonly string _multipleEquipmentRepositoriesDataFolderPath =
        Path.Combine("Data", "CivilianEquipmentPoolProvider", "MultipleRepos");

    private readonly string _firstEquipmentRepositoryDataFolderPath =
        Path.Combine("Data", "CivilianEquipmentPoolProvider", "MultipleRepos", "FirstRepo");

    private readonly string _secondEquipmentRepositoryDataFolderPath =
        Path.Combine("Data", "CivilianEquipmentPoolProvider", "MultipleRepos", "SecondRepo");

    private Mock<ICacheProvider> _cacheProvider;
    private Mock<ILogger> _logger;
    private Mock<ILoggerFactory> _loggerFactory;

    [SetUp]
    public void SetUp()
    {
        _cacheProvider = new Mock<ICacheProvider>();
        _logger = new Mock<ILogger>();
        _loggerFactory = new Mock<ILoggerFactory>();
        _loggerFactory.Setup(factory => factory.CreateLogger<CivilianEquipmentPoolProvider>())
            .Returns(_logger.Object);
    }

    [Test]
    public void GetEquipmentPoolsFromSingleRepository()
    {
        var firstEquipmentRepository =
            CreateEquipmentRepository(InputFolder(_validSiegeEquipmentDataFolderPath));
        var troopEquipmentReader =
            new CivilianEquipmentPoolProvider(_loggerFactory.Object, _cacheProvider.Object, firstEquipmentRepository);

        var allTroopEquipmentPools = troopEquipmentReader.GetCivilianEquipmentByCharacterAndPool();

        AssertCharacterEquipmentPools(ExpectedFolder(_validSiegeEquipmentDataFolderPath), allTroopEquipmentPools);
    }

    [Test]
    public void GetEquipmentPoolsFromMultipleRepositories()
    {
        var firstEquipmentRepository =
            CreateEquipmentRepository(InputFolder(_firstEquipmentRepositoryDataFolderPath));
        var secondEquipmentRepository =
            CreateEquipmentRepository(InputFolder(_secondEquipmentRepositoryDataFolderPath));
        var troopEquipmentReader =
            new CivilianEquipmentPoolProvider(_loggerFactory.Object, _cacheProvider.Object, firstEquipmentRepository,
                secondEquipmentRepository);

        var allTroopEquipmentPools = troopEquipmentReader.GetCivilianEquipmentByCharacterAndPool();

        AssertCharacterEquipmentPools(ExpectedFolder(_multipleEquipmentRepositoriesDataFolderPath),
            allTroopEquipmentPools);
        _logger.Verify(
            logger => logger.Warn(
                "'vlandian_recruit' is defined in multiple xml files. Only the first equipment list will be used.",
                null),
            Times.Once);
    }

    [Test]
    public void NotReturnEquipmentPools_WhenInvalidSymbolsAreUsedInCondition()
    {
        var recruitId = "vlandian_recruit";
        var characterEquipmentRepository =
            CreateEquipmentRepository(InputFolder(_invalidSiegeEquipmentDataFolderPath));
        var troopEquipmentReader =
            new CivilianEquipmentPoolProvider(_loggerFactory.Object, _cacheProvider.Object,
                characterEquipmentRepository);

        var allTroopEquipmentPools = troopEquipmentReader.GetCivilianEquipmentByCharacterAndPool();

        Assert.That(allTroopEquipmentPools, Is.Not.Null);
        Assert.That(allTroopEquipmentPools.Count, Is.EqualTo(1));
        Assert.That(allTroopEquipmentPools[recruitId].Count, Is.EqualTo(0));
    }

    [Test]
    public void GetCachedEquipmentPools()
    {
        var characterEquipmentRepository =
            CreateEquipmentRepository(InputFolder(_invalidSiegeEquipmentDataFolderPath));
        var troopEquipmentReader =
            new CivilianEquipmentPoolProvider(_loggerFactory.Object, _cacheProvider.Object,
                characterEquipmentRepository);

        _cacheProvider.Setup(provider => provider.CacheObject(It.IsAny<object>()))
            .Returns(CachedObjectId);
        _cacheProvider.Setup(provider =>
            provider.InvalidateCache(CachedObjectId, CampaignEvent.OnAfterSessionLaunched));

        var allTroopEquipmentPools = troopEquipmentReader.GetCivilianEquipmentByCharacterAndPool();

        _cacheProvider.Setup(cacheProvider =>
                cacheProvider.GetCachedObject<IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>>(
                    CachedObjectId))
            .Returns(allTroopEquipmentPools);

        var cachedAllTroopEquipmentPools = troopEquipmentReader.GetCivilianEquipmentByCharacterAndPool();

        _cacheProvider.VerifyAll();
        Assert.That(cachedAllTroopEquipmentPools, Is.EqualTo(allTroopEquipmentPools));
    }

    private IEquipmentPoolsRepository CreateEquipmentRepository(string inputFolderPath)
    {
        var characterEquipmentRepository = new Mock<IEquipmentPoolsRepository>();
        characterEquipmentRepository
            .Setup(repository => repository.GetEquipmentPoolsById())
            .Returns(ReadEquipmentPoolFromDataFolder(inputFolderPath));
        return characterEquipmentRepository.Object;
    }
}
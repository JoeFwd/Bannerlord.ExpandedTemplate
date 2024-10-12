using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.Siege;
using Moq;
using NUnit.Framework;
using static Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.TestUtil.TestFolderComparator;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Providers.Siege;

public class SiegeEquipmentPoolProviderShould
{
    private const string CachedObjectId = "irrelevant_cached_object_id";

    private readonly string _validSiegeEquipmentDataFolderPath =
        Path.Combine("Data", "SiegeEquipmentPoolProvider", "ValidSymbols");

    private readonly string _invalidSiegeEquipmentDataFolderPath =
        Path.Combine("Data", "SiegeEquipmentPoolProvider", "InvalidSymbols");

    private readonly string _multipleEquipmentRepositoriesDataFolderPath =
        Path.Combine("Data", "SiegeEquipmentPoolProvider", "MultipleRepos");

    private readonly string _firstEquipmentRepositoryDataFolderPath =
        Path.Combine("Data", "SiegeEquipmentPoolProvider", "MultipleRepos", "FirstRepo");

    private readonly string _secondEquipmentRepositoryDataFolderPath =
        Path.Combine("Data", "SiegeEquipmentPoolProvider", "MultipleRepos", "SecondRepo");

    private Mock<ICacheProvider> _cacheProvider;
    private Mock<ILogger> _logger;
    private Mock<ILoggerFactory> _loggerFactory;

    [SetUp]
    public void SetUp()
    {
        _cacheProvider = new Mock<ICacheProvider>();
        _logger = new Mock<ILogger>();
        _loggerFactory = new Mock<ILoggerFactory>();
        _loggerFactory.Setup(factory => factory.CreateLogger<SiegeEquipmentPoolProvider>())
            .Returns(_logger.Object);
    }

    [Test]
    public void GetEquipmentPoolsFromSingleRepository()
    {
        var characterEquipmentRepository =
            CreateEquipmentRepository(InputFolder(_validSiegeEquipmentDataFolderPath));
        var troopEquipmentReader =
            new SiegeEquipmentPoolProvider(_loggerFactory.Object, _cacheProvider.Object, characterEquipmentRepository);
        
        var allTroopEquipmentPools = troopEquipmentReader.GetSiegeEquipmentByCharacterAndPool();

        AssertCharacterEquipmentPools(ExpectedFolder(_validSiegeEquipmentDataFolderPath),
            allTroopEquipmentPools);
    }

    [Test]
    public void GetEquipmentPoolsFromMultipleRepositories()
    {
        var firstEquipmentRepository =
            CreateEquipmentRepository(InputFolder(_firstEquipmentRepositoryDataFolderPath));
        var secondEquipmentRepository =
            CreateEquipmentRepository(InputFolder(_secondEquipmentRepositoryDataFolderPath));
        var troopEquipmentReader =
            new SiegeEquipmentPoolProvider(_loggerFactory.Object, _cacheProvider.Object, firstEquipmentRepository,
                secondEquipmentRepository);

        _cacheProvider.Setup(provider => provider.CacheObject(It.IsAny<object>()))
            .Returns(CachedObjectId);
        _cacheProvider.Setup(provider =>
            provider.InvalidateCache(CachedObjectId, CampaignEvent.OnAfterSessionLaunched));

        var allTroopEquipmentPools = troopEquipmentReader.GetSiegeEquipmentByCharacterAndPool();

        AssertCharacterEquipmentPools(ExpectedFolder(_multipleEquipmentRepositoriesDataFolderPath),
            allTroopEquipmentPools);
        _logger.Verify(
            logger => logger.Warn(
                "'vlandian_recruit' is defined in multiple xml files. Only the first equipment list will be used.",
                null),
            Times.Once);
    }

    [Test]
    public void GettingSiegeEquipmentPools_WithInvalidSiegeFlags_DoesNotAddEquipmentToPools()
    {
        var recruitId = "vlandian_recruit";
        var characterEquipmentRepository =
            CreateEquipmentRepository(InputFolder(_invalidSiegeEquipmentDataFolderPath));
        var troopEquipmentReader =
            new SiegeEquipmentPoolProvider(_loggerFactory.Object, _cacheProvider.Object, characterEquipmentRepository);

        var allTroopEquipmentPools = troopEquipmentReader.GetSiegeEquipmentByCharacterAndPool();

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
            new SiegeEquipmentPoolProvider(_loggerFactory.Object, _cacheProvider.Object, characterEquipmentRepository);

        _cacheProvider.Setup(provider => provider.CacheObject(It.IsAny<object>()))
            .Returns(CachedObjectId);
        _cacheProvider.Setup(provider =>
            provider.InvalidateCache(CachedObjectId, CampaignEvent.OnAfterSessionLaunched));

        var allTroopEquipmentPools = troopEquipmentReader.GetSiegeEquipmentByCharacterAndPool();

        _cacheProvider.Setup(cacheProvider =>
                cacheProvider.GetCachedObject<IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>>(
                    CachedObjectId))
            .Returns(allTroopEquipmentPools);

        var cachedAllTroopEquipmentPools = troopEquipmentReader.GetSiegeEquipmentByCharacterAndPool();

        _cacheProvider.VerifyAll();
        Assert.That(cachedAllTroopEquipmentPools, Is.EqualTo(allTroopEquipmentPools));
    }

    private IEquipmentPoolsRepository CreateEquipmentRepository(string inputFolderPath)
    {
        var characterEquipmentRepository = new Mock<IEquipmentPoolsRepository>(MockBehavior.Strict);
        characterEquipmentRepository
            .Setup(repository => repository.GetEquipmentPoolsById())
            .Returns(ReadEquipmentPoolFromDataFolder(inputFolderPath));
        return characterEquipmentRepository.Object;
    }
}
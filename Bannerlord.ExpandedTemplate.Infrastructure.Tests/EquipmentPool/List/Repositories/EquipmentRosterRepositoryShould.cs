using System.Xml.Linq;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Xml;
using Bannerlord.ExpandedTemplate.Infrastructure.Exception;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Repositories;

public class EquipmentRosterRepositoryShould
{
    private const string CachedObjectId = "irrelevant_cached_object_id";

    private Mock<IXmlProcessor> _xmlProcessor;
    private Mock<ICachingProvider> _cacheProvider;
    private Mock<ILoggerFactory> _loggerFactory;
    private Mock<IEquipmentRostersReader> _rostersReaderMock; // <- added mock field
    private IEquipmentRosterRepository _equipmentRosterRepository;
    
    [SetUp]
    public void Setup()
    {
        _xmlProcessor = new Mock<IXmlProcessor>(MockBehavior.Strict);
        _cacheProvider = new Mock<ICachingProvider>(MockBehavior.Strict);
        _loggerFactory = new Mock<ILoggerFactory>(MockBehavior.Strict);
        _loggerFactory.Setup(factory => factory.CreateLogger<IEquipmentRosterRepository>())
            .Returns(new Mock<ILogger>(MockBehavior.Strict).Object);

        _rostersReaderMock = new Mock<IEquipmentRostersReader>(MockBehavior.Strict);


        _equipmentRosterRepository =
            new EquipmentRosterRepository(
                _xmlProcessor.Object,
                _rostersReaderMock.Object,
                _cacheProvider.Object,
                _loggerFactory.Object);
    }

    [Test]
    public void GetEquipmentRosters()
    {
        string xml = "<EquipmentRosters />";

        var equipmentRosters = new List<EquipmentRoster> { new() };
        _rostersReaderMock.Setup(r => r.ReadAll(xml))
            .Returns((string xml) => equipmentRosters);

        _xmlProcessor.Setup(processor => processor.GetXmlNodes(EquipmentRosterRepository.EquipmentRostersRootTag))
            .Returns(XDocument.Parse(xml));
        _cacheProvider.Setup(provider => provider.CacheObject(It.IsAny<EquipmentRosters>(), CacheDataType.Xml))
            .Returns(CachedObjectId);

        EquipmentRosters actualEquipmentRosters = _equipmentRosterRepository.GetEquipmentRosters();

        Assert.That(actualEquipmentRosters,
            Is.EqualTo(new EquipmentRosters { EquipmentRoster = equipmentRosters }));
    }

    [Test]
    public void GetCachedEquipmentRosters_ReturnsCachedResult()
    {
        string xml = "<EquipmentRosters />";
        var equipmentRosters = new List<EquipmentRoster>();
        _rostersReaderMock.Setup(r => r.ReadAll(xml))
            .Returns((string xml) => equipmentRosters);
        _xmlProcessor.Setup(p => p.GetXmlNodes(EquipmentRosterRepository.EquipmentRostersRootTag))
            .Returns(XDocument.Parse(xml));
        _cacheProvider.Setup(c => c.CacheObject(It.IsAny<EquipmentRosters>(), CacheDataType.Xml))
            .Returns(CachedObjectId);

        _equipmentRosterRepository.GetEquipmentRosters();

        var cachedEquipmentRosters = new EquipmentRosters { EquipmentRoster = new List<EquipmentRoster> { new() } };
        _cacheProvider.Setup(c => c.GetObject<EquipmentRosters>(CachedObjectId))
            .Returns(cachedEquipmentRosters);

        var secondResult = _equipmentRosterRepository.GetEquipmentRosters();

        Assert.That(secondResult, Is.EqualTo(cachedEquipmentRosters));
        _cacheProvider.VerifyAll();
    }

    [Test]
    public void ThrowsTechnicalException_WhenIOErrorOccurs()
    {
        _xmlProcessor
            .Setup(processor => processor.GetXmlNodes(EquipmentRosterRepository.EquipmentRostersRootTag))
            .Throws(new IOException());

        System.Exception? ex = Assert.Throws<TechnicalException>(() => _equipmentRosterRepository.GetEquipmentRosters());
        Assert.That(ex?.Message, Is.EqualTo(EquipmentRosterRepository.IoErrorMessage));
    }

    [Test]
    public void ThrowsTechnicalException_WhenEquipmentRostersAreNotDefinedCorrectly()
    {
        _xmlProcessor
            .Setup(processor => processor.GetXmlNodes(EquipmentRosterRepository.EquipmentRostersRootTag))
            .Returns(XDocument.Parse("<InvalidRootTag/>"));

        System.Exception? ex = Assert.Throws<TechnicalException>(() => _equipmentRosterRepository.GetEquipmentRosters());
        Assert.That(ex?.Message, Is.EqualTo(EquipmentRosterRepository.DeserialisationErrorMessage));
    }
}
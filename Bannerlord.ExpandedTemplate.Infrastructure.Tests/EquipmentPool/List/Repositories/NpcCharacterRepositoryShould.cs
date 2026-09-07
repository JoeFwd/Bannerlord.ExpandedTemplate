using System.IO;
using System.Xml.Linq;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories;
using Bannerlord.ExpandedTemplate.Infrastructure.Exception;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Repositories;

public class NpcCharacterRepositoryShould
{
    private const string CachedObjectId = "cached";
    private Mock<ICachingProvider> _cacheProvider = null!;
    private INpcCharacterRepository _repository = null!;
    private Mock<IXmlProcessor> _xmlProcessor = null!;

    [SetUp]
    public void SetUp()
    {
        _xmlProcessor = new Mock<IXmlProcessor>(MockBehavior.Strict);
        _cacheProvider = new Mock<ICachingProvider>(MockBehavior.Strict);
        var loggerFactory = new Mock<ILoggerFactory>(MockBehavior.Strict);
        loggerFactory.Setup(factory => factory.CreateLogger<INpcCharacterRepository>()).Returns(new Mock<ILogger>().Object);
        _repository = new NpcCharacterRepository(_xmlProcessor.Object, _cacheProvider.Object, loggerFactory.Object);
    }

    [Test]
    public void DeserializeAndCacheNpcCharacters()
    {
        _xmlProcessor.Setup(processor => processor.GetXmlNodes(NpcCharacterRepository.NpcCharacterRootTag))
            .Returns(XDocument.Parse("<NPCCharacters><NPCCharacter id=\"character\"><Equipments><EquipmentRoster pool=\"pool\"><equipment slot=\"head\" id=\"helmet\" /></EquipmentRoster></Equipments></NPCCharacter></NPCCharacters>"));
        _cacheProvider.Setup(cache => cache.CacheObject(It.IsAny<NpcCharacters>(), CacheDataType.Xml)).Returns(CachedObjectId);

        NpcCharacters characters = _repository.GetNpcCharacters();

        Assert.That(characters.NpcCharacter, Has.Count.EqualTo(1));
        Assert.That(characters.NpcCharacter[0].Id, Is.EqualTo("character"));
        Assert.That(characters.NpcCharacter[0].Equipments.EquipmentRoster[0].Equipment[0].Id, Is.EqualTo("helmet"));
    }

    [Test]
    public void ReturnCachedNpcCharacters()
    {
        _xmlProcessor.Setup(processor => processor.GetXmlNodes(NpcCharacterRepository.NpcCharacterRootTag))
            .Returns(XDocument.Parse("<NPCCharacters />"));
        _cacheProvider.Setup(cache => cache.CacheObject(It.IsAny<NpcCharacters>(), CacheDataType.Xml)).Returns(CachedObjectId);
        NpcCharacters cached = new() { NpcCharacter = new List<NpcCharacter> { new() { Id = "cached" } } };
        _cacheProvider.Setup(cache => cache.GetObject<NpcCharacters>(CachedObjectId)).Returns(cached);

        _repository.GetNpcCharacters();
        NpcCharacters actual = _repository.GetNpcCharacters();

        Assert.That(actual, Is.SameAs(cached));
    }

    [TestCase(typeof(IOException), NpcCharacterRepository.IoErrorMessage)]
    [TestCase(typeof(InvalidOperationException), NpcCharacterRepository.DeserialisationErrorMessage)]
    public void WrapLoadingErrors(Type exceptionType, string expectedMessage)
    {
        _xmlProcessor.Setup(processor => processor.GetXmlNodes(NpcCharacterRepository.NpcCharacterRootTag))
            .Throws((System.Exception)System.Activator.CreateInstance(exceptionType)!);

        TechnicalException exception = Assert.Throws<TechnicalException>(() => _repository.GetNpcCharacters())!;

        Assert.That(exception.Message, Is.EqualTo(expectedMessage));
    }
}

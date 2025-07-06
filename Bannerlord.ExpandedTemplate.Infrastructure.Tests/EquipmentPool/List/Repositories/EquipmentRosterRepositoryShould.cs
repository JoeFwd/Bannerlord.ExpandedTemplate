﻿using System.Xml.Linq;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories;
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
    private IEquipmentRosterRepository _equipmentRosterRepository;

    [SetUp]
    public void Setup()
    {
        _xmlProcessor = new Mock<IXmlProcessor>(MockBehavior.Strict);
        _cacheProvider = new Mock<ICachingProvider>(MockBehavior.Strict);
        _loggerFactory = new Mock<ILoggerFactory>(MockBehavior.Strict);
        _loggerFactory.Setup(factory => factory.CreateLogger<IEquipmentRosterRepository>())
            .Returns(new Mock<ILogger>(MockBehavior.Strict).Object);
        _equipmentRosterRepository =
            new EquipmentRosterRepository(_xmlProcessor.Object, _cacheProvider.Object, _loggerFactory.Object);
    }

    [Test]
    public void GetEquipmentRosters()
    {
        string xml =
            """
            <EquipmentRosters>
                <EquipmentRoster id="irrelevant_equipment_roster_id" culture="irrelevant_culture_id">
                    <EquipmentSet civilian="irrelevant_civilian_flag" siege="irrelevant_siege_flag" pool="irrelevant_pool_id">
                        <Equipment slot="irrelevant_slot_1" id="irrelevant_slot_id_1"/>
                        <Equipment slot="irrelevant_slot_2" id="irrelevant_slot_id_2"/>
                    </EquipmentSet>
                    <EquipmentSet/>
                </EquipmentRoster>
                <EquipmentRoster id="irrelevant_empty_equipment_roster_id"/>
            </EquipmentRosters>
            """;

        _xmlProcessor.Setup(processor => processor.GetXmlNodes(EquipmentRosterRepository.EquipmentRostersRootTag))
            .Returns(XDocument.Parse(xml));
        _cacheProvider.Setup(provider => provider.CacheObject(It.IsAny<EquipmentRosters>(), CacheDataType.Xml))
            .Returns(CachedObjectId);

        EquipmentRosters equipmentRosters = _equipmentRosterRepository.GetEquipmentRosters();

        Assert.That(equipmentRosters, Is.EqualTo(new EquipmentRosters
        {
            EquipmentRoster = new List<EquipmentRoster>
            {
                new()
                {
                    Id = "irrelevant_equipment_roster_id",
                    EquipmentSet = new List<EquipmentSet>
                    {
                        new()
                        {
                            Equipment = new List<Equipment>
                            {
                                new()
                                {
                                    Slot = "irrelevant_slot_1",
                                    Id = "irrelevant_slot_id_1"
                                },
                                new()
                                {
                                    Slot = "irrelevant_slot_2",
                                    Id = "irrelevant_slot_id_2"
                                }
                            },
                            Pool = "irrelevant_pool_id",
                            IsCivilian = "irrelevant_civilian_flag",
                            IsSiege = "irrelevant_siege_flag"
                        },
                        new()
                    }
                },
                new()
                {
                    Id = "irrelevant_empty_equipment_roster_id"
                }
            }
        }));
    }

    [Test]
    public void GetCachedNpcCharacters()
    {
        string xml = "<EquipmentRosters/>";

        _xmlProcessor.Setup(processor => processor.GetXmlNodes(EquipmentRosterRepository.EquipmentRostersRootTag))
            .Returns(XDocument.Parse(xml));
        _cacheProvider.Setup(provider => provider.CacheObject(It.IsAny<EquipmentRosters>(), CacheDataType.Xml))
            .Returns(CachedObjectId);

        // First call to populate cache
        var equipmentRosters = _equipmentRosterRepository.GetEquipmentRosters();

        _cacheProvider.Verify(provider => provider.CacheObject(equipmentRosters, CacheDataType.Xml), Times.Once);

        _cacheProvider.Setup(cache => cache.GetObject<EquipmentRosters>(CachedObjectId))
            .Returns(new EquipmentRosters());

        EquipmentRosters cachedEquipmentRosters = _equipmentRosterRepository.GetEquipmentRosters();

        _cacheProvider.VerifyAll();
        Assert.That(cachedEquipmentRosters, Is.EqualTo(equipmentRosters));
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
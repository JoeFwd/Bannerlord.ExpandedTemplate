using System.Xml.Linq;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories;
using Bannerlord.ExpandedTemplate.Infrastructure.Exception;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Repositories;

public class NpcRepositoryRepositoryShould
{
    private const string CachedObjectId = "irrelevant_cached_object_id";
    
    private Mock<IXmlProcessor> _xmlProcessor;
    private Mock<ICachingProvider> _cacheProvider;
    private Mock<ILoggerFactory> _loggerFactory;
    private INpcCharacterRepository _npcCharacterRepository;

    [SetUp]
    public void Setup()
    {
        _xmlProcessor = new Mock<IXmlProcessor>(MockBehavior.Strict);
        _cacheProvider = new Mock<ICachingProvider>(MockBehavior.Strict);
        _loggerFactory = new Mock<ILoggerFactory>(MockBehavior.Strict);
        _loggerFactory.Setup(factory => factory.CreateLogger<INpcCharacterRepository>())
            .Returns(new Mock<ILogger>(MockBehavior.Strict).Object);
        _npcCharacterRepository =
            new NpcCharacterRepository(_xmlProcessor.Object, _cacheProvider.Object, _loggerFactory.Object);
    }

    [Test]
    public void GetNpcCharacters()
    {
        string xml =
            """
            <NPCCharacters>
                <NPCCharacter id="irrelevant_character_id_1" culture="irrelevant_culture_id">
                    <Equipments>
                         <EquipmentRoster/>
                        <EquipmentSet/>
                    </Equipments>
                </NPCCharacter>
                <NPCCharacter id="irrelevant_character_id_2" culture="irrelevant_culture_id">
                    <Equipments>
                        <EquipmentRoster pool="irrelevant_pool_id">
                            <equipment slot="irrelevant_slot_1" id="irrelevant_slot_id_1"/>
                            <equipment slot="irrelevant_slot_2" id="irrelevant_slot_id_2"/>
                        </EquipmentRoster>
                        <EquipmentSet pool="irrelevant_pool_id" id="irrelevant_equipment_set_id"
                            civilian="irrelevant_civilian_flag" siege="irrelevant_siege_flag" />
                    </Equipments>
                </NPCCharacter>
            </NPCCharacters>
            """;

        _xmlProcessor.Setup(processor => processor.GetXmlNodes(NpcCharacterRepository.NpcCharacterRootTag))
            .Returns(XDocument.Parse(xml));
        _cacheProvider.Setup(cache => cache.CacheObject(It.IsAny<NpcCharacters>(), CacheDataType.Xml))
            .Returns(CachedObjectId);

        NpcCharacters equipmentRosters = _npcCharacterRepository.GetNpcCharacters();

        Assert.That(equipmentRosters, Is.EqualTo(new NpcCharacters
        {
            NpcCharacter = new List<NpcCharacter>
            {
                new()
                {
                    Id = "irrelevant_character_id_1",
                    Equipments = new Equipments
                    {
                        EquipmentRoster = new List<EquipmentRoster>
                        {
                            new()
                        },
                        EquipmentSet = new List<EquipmentSet>
                        {
                            new()
                        }
                    }
                },
                new()
                {
                    Id = "irrelevant_character_id_2",
                    Equipments = new Equipments
                    {
                        EquipmentRoster = new List<EquipmentRoster>
                        {
                            new()
                            {
                                Pool = "irrelevant_pool_id",
                                Equipment = new List<Equipment>
                                {
                                    new() { Slot = "irrelevant_slot_1", Id = "irrelevant_slot_id_1" },
                                    new() { Slot = "irrelevant_slot_2", Id = "irrelevant_slot_id_2" }
                                }
                            }
                        },
                        EquipmentSet = new List<EquipmentSet>
                        {
                            new()
                            {
                                Pool = "irrelevant_pool_id",
                                Id = "irrelevant_equipment_set_id",
                                IsCivilian = "irrelevant_civilian_flag",
                                IsSiege = "irrelevant_siege_flag"
                            }
                        }
                    }
                }
            }
        }));
    }

    [Test]
    public void MergeNpcCharacters_IfDefinedMultipleTimes()
    {
        string xml =
            """
            <NPCCharacters>
                <NPCCharacter id="irrelevant_character_id_1" culture="irrelevant_culture_id">
                    <Equipments>
                        <EquipmentRoster>
                            <equipment slot="irrelevant_slot_1" id="irrelevant_slot_id_1"/>
                        </EquipmentRoster>
                        <EquipmentRoster>
                            <equipment slot="irrelevant_slot_2" id="irrelevant_slot_id_2"/>
                        </EquipmentRoster>
                        <EquipmentSet id="irrelevant_equipment_set_id1" />
                        <EquipmentSet id="irrelevant_equipment_set_id2" />                
                    </Equipments>
                </NPCCharacter>
                <NPCCharacter id="irrelevant_character_id_1" culture="irrelevant_culture_id">
                    <Equipments>
                        <EquipmentRoster>
                            <equipment slot="irrelevant_slot_2" id="irrelevant_slot_id_2"/>
                        </EquipmentRoster>
                        <EquipmentRoster>
                            <equipment slot="irrelevant_slot_3" id="irrelevant_slot_id_3"/>
                        </EquipmentRoster>
                        <EquipmentSet id="irrelevant_equipment_set_id2" />
                        <EquipmentSet id="irrelevant_equipment_set_id3" />                
                    </Equipments>
                </NPCCharacter>
            </NPCCharacters>
            """;

        _xmlProcessor.Setup(processor => processor.GetXmlNodes(NpcCharacterRepository.NpcCharacterRootTag))
            .Returns(XDocument.Parse(xml));
        _cacheProvider.Setup(cache => cache.CacheObject(It.IsAny<NpcCharacters>(), CacheDataType.Xml))
            .Returns(CachedObjectId);

        NpcCharacters equipmentRosters = _npcCharacterRepository.GetNpcCharacters();

        Assert.That(equipmentRosters, Is.EqualTo(new NpcCharacters
        {
            NpcCharacter = new List<NpcCharacter>
            {
                new()
                {
                    Id = "irrelevant_character_id_1",
                    Equipments = new Equipments
                    {
                        EquipmentRoster = new List<EquipmentRoster>
                        {
                            new()
                            {
                                Equipment = new List<Equipment>
                                {
                                    new() { Slot = "irrelevant_slot_1", Id = "irrelevant_slot_id_1" }
                                }
                            },
                            new()
                            {
                                Equipment = new List<Equipment>
                                {
                                    new() { Slot = "irrelevant_slot_2", Id = "irrelevant_slot_id_2" }
                                }
                            },
                            new()
                            {
                                Equipment = new List<Equipment>
                                {
                                    new() { Slot = "irrelevant_slot_2", Id = "irrelevant_slot_id_2" }
                                }
                            },
                            new()
                            {
                                Equipment = new List<Equipment>
                                {
                                    new() { Slot = "irrelevant_slot_3", Id = "irrelevant_slot_id_3" }
                                }
                            }
                        },
                        EquipmentSet = new List<EquipmentSet>
                        {
                            new()
                            {
                                Id = "irrelevant_equipment_set_id1"
                            },
                            new()
                            {
                                Id = "irrelevant_equipment_set_id2"
                            },
                            new()
                            {
                                Id = "irrelevant_equipment_set_id2"
                            },
                            new()
                            {
                                Id = "irrelevant_equipment_set_id3"
                            }
                        }
                    }
                }
            }
        }));
    }

    
    [Test]
    public void GetCachedNpcCharacters()
    {
        string xml = "<NPCCharacters/>";

        _xmlProcessor.Setup(processor => processor.GetXmlNodes(NpcCharacterRepository.NpcCharacterRootTag))
            .Returns(XDocument.Parse(xml));
        _cacheProvider.Setup(cache => cache.CacheObject(It.IsAny<NpcCharacters>(), CacheDataType.Xml))
            .Returns(CachedObjectId);

        var equipmentRosters = _npcCharacterRepository.GetNpcCharacters();

        _cacheProvider.Verify(provider => provider.CacheObject(equipmentRosters, CacheDataType.Xml), Times.Once);
        var cachedNpcCharacters = new NpcCharacters { NpcCharacter = new List<NpcCharacter> { new() } };
        _cacheProvider.Setup(cache => cache.GetObject<NpcCharacters>(CachedObjectId))
            .Returns(cachedNpcCharacters);

        NpcCharacters actualCachedNpcCharacters = _npcCharacterRepository.GetNpcCharacters();

        _cacheProvider.VerifyAll();
        Assert.That(actualCachedNpcCharacters, Is.EqualTo(cachedNpcCharacters));
    }

    [Test]
    public void ThrowsTechnicalException_WhenIOErrorOccurs()
    {
        _xmlProcessor
            .Setup(processor => processor.GetXmlNodes(NpcCharacterRepository.NpcCharacterRootTag))
            .Throws(new IOException());

        System.Exception? ex = Assert.Throws<TechnicalException>(() => _npcCharacterRepository.GetNpcCharacters());
        Assert.That(ex?.Message, Is.EqualTo(NpcCharacterRepository.IoErrorMessage));
    }

    [Test]
    public void ThrowsTechnicalException_WhenNpcCharactersAreNotDefinedCorrectly()
    {
        _xmlProcessor
            .Setup(processor => processor.GetXmlNodes(NpcCharacterRepository.NpcCharacterRootTag))
            .Returns(XDocument.Parse("<InvalidRootTag/>"));

        System.Exception? ex = Assert.Throws<TechnicalException>(() => _npcCharacterRepository.GetNpcCharacters());
        Assert.That(ex?.Message, Is.EqualTo(NpcCharacterRepository.DeserialisationErrorMessage));
    }
}
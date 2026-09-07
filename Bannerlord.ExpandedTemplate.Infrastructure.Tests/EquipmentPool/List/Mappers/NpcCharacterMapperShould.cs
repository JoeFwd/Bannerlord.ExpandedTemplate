using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories;
using Moq;
using NUnit.Framework;
using Equipment = Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters.Equipment;
using EquipmentRoster = Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters.EquipmentRoster;
using EquipmentSet = Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters.EquipmentSet;
using Roster = Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters.EquipmentRoster;
using RosterEquipmentSet =
    Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters.EquipmentSet;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Mappers;

public class NpcCharacterMapperShould
{
    private Mock<IEquipmentRosterRepository> _equipmentRosterRepository;
    private Mock<IEquipmentSetMapper> _equipmentRosterMapper;
    private INpcCharacterMapper _npcCharacterMapper;

    [SetUp]
    public void SetUp()
    {
        _equipmentRosterRepository = new Mock<IEquipmentRosterRepository>(MockBehavior.Strict);
        _equipmentRosterMapper = new Mock<IEquipmentSetMapper>(MockBehavior.Strict);
        var loggerFactory = new Mock<ILoggerFactory>(MockBehavior.Strict);
        loggerFactory.Setup(factory => factory.CreateLogger<NpcCharacterMapper>()).Returns(new Mock<ILogger>().Object);
        _npcCharacterMapper = new NpcCharacterMapper(
            _equipmentRosterRepository.Object,
            _equipmentRosterMapper.Object,
            new EquipmentSetFlagMapper(),
            loggerFactory.Object);
    }

    [Test]
    public void MapsEquipmentRoster()
    {
        var npcCharacter = new NpcCharacter
        {
            Id = "npc1",
            Equipments = new Equipments
            {
                EquipmentRoster = new List<EquipmentRoster>
                {
                    new()
                    {
                        Pool = "0",
                        Equipment = new List<Equipment> { new() { Slot = "Arm", Id = "item1" } }
                    }
                }
            }
        };
        SetEquipmentRosters();

        IList<EquipmentRoster> equipmentPools = _npcCharacterMapper.MapToEquipmentRosters(npcCharacter);

        Assert.That(equipmentPools, Is.EqualTo(npcCharacter.Equipments.EquipmentRoster));
    }

    [Test]
    public void MapsReferencedEquipmentSet()
    {
        var referencedSet = new RosterEquipmentSet();
        var mappedRoster = new EquipmentRoster { Pool = "0" };
        var npcCharacter = new NpcCharacter
        {
            Id = "npc1",
            Equipments = new Equipments { EquipmentSet = new List<EquipmentSet> { new() { Id = "set1" } } }
        };
        SetEquipmentRosters(new Roster
        {
            Id = "set1",
            EquipmentSet = new List<RosterEquipmentSet> { referencedSet }
        });
        _equipmentRosterMapper.Setup(mapper => mapper.MapToEquipmentRoster(referencedSet)).Returns(mappedRoster);

        IList<EquipmentRoster> equipmentRosters = _npcCharacterMapper.MapToEquipmentRosters(npcCharacter);

        Assert.That(equipmentRosters, Is.EqualTo(new List<EquipmentRoster> { mappedRoster }));
    }

    [Test]
    public void OverridesEquipmentRosterSlotsWithRootEquipmentSlots()
    {
        var referencedSet = new RosterEquipmentSet
        {
            Equipment = new List<Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters.Equipment>
            {
                new() { Slot = "Arm", Id = "item1" }
            }
        };
        var npcCharacter = new NpcCharacter
        {
            Id = "npc1",
            Equipments = new Equipments
            {
                EquipmentRoster = new List<EquipmentRoster>
                {
                    new()
                    {
                        Pool = "0",
                        Equipment = new List<Equipment>
                        {
                            new() { Slot = "Arm", Id = "item0" }, new() { Slot = "Body", Id = "item1" }
                        }
                    }
                },
                EquipmentSet = new List<EquipmentSet> { new() { Id = "set1" } },
                Equipment = new List<Equipment>
                {
                    new() { Slot = "Arm", Id = "item4" }, new() { Slot = "Item0", Id = "item0" }
                }
            }
        };
        SetEquipmentRosters(new Roster
        {
            Id = "set1",
            EquipmentSet = new List<RosterEquipmentSet> { referencedSet }
        });
        _equipmentRosterMapper.Setup(mapper => mapper.MapToEquipmentRoster(referencedSet)).Returns(new EquipmentRoster
        {
            Pool = "0",
            Equipment = new List<Equipment> { new() { Slot = "Arm", Id = "item1" } }
        });

        IList<EquipmentRoster> equipmentPools = _npcCharacterMapper.MapToEquipmentRosters(npcCharacter);

        Assert.That(equipmentPools, Is.EquivalentTo(new List<EquipmentRoster>
        {
            new()
            {
                Pool = "0",
                Equipment = new List<Equipment>
                {
                    new() { Slot = "Arm", Id = "item4" }, new() { Slot = "Body", Id = "item1" },
                    new() { Slot = "Item0", Id = "item0" }
                }
            },
            new()
            {
                Pool = "0",
                Equipment = new List<Equipment>
                {
                    new() { Slot = "Arm", Id = "item4" }, new() { Slot = "Item0", Id = "item0" }
                }
            }
        }));
    }

    [Test]
    public void RemovesEmptyEquipmentRoster()
    {
        var npcCharacter = new NpcCharacter
        {
            Id = "npc1",
            Equipments = new Equipments { EquipmentRoster = new List<EquipmentRoster> { new() } }
        };
        SetEquipmentRosters();

        IList<EquipmentRoster> equipmentPools = _npcCharacterMapper.MapToEquipmentRosters(npcCharacter);

        Assert.That(equipmentPools, Is.Empty);
    }

    [Test]
    public void RemovesEmptyEquipmentRosterFromReferencedEquipmentRosters()
    {
        var npcCharacter = new NpcCharacter
        {
            Id = "npc1",
            Equipments = new Equipments { EquipmentSet = new List<EquipmentSet> { new() { Id = "set1" } } }
        };
        SetEquipmentRosters(new Roster
        {
            Id = "set1",
            EquipmentSet = new List<RosterEquipmentSet>()
        });

        IList<EquipmentRoster> equipmentPools = _npcCharacterMapper.MapToEquipmentRosters(npcCharacter);

        Assert.That(equipmentPools, Is.Empty);
    }

    private void SetEquipmentRosters(params Roster[] equipmentRosters)
    {
        _equipmentRosterRepository.Setup(repository => repository.GetEquipmentRosters()).Returns(new EquipmentRosters
        {
            EquipmentRoster = equipmentRosters.ToList()
        });
    }
}

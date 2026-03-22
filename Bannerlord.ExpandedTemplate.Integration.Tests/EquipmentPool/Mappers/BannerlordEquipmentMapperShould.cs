using System;
using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.Mappers;
using Moq;
using NUnit.Framework;
using TaleWorlds.Core;
using Equipment = TaleWorlds.Core.Equipment;

namespace Bannerlord.ExpandedTemplate.Integration.Tests.EquipmentPool.Mappers;

[TestFixture]
public class BannerlordEquipmentMapperShould
{
    private Mock<ILoggerFactory> _loggerFactoryMock = null!;
    private BannerlordEquipmentMapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerFactoryMock = new Mock<ILoggerFactory>();
        var loggerMock = new Mock<ILogger>();
        _loggerFactoryMock.Setup(f => f.CreateLogger<BannerlordEquipmentMapper>()).Returns(loggerMock.Object);
        _mapper = new BannerlordEquipmentMapper(_loggerFactoryMock.Object);
    }

    [Test]
    public void Handle_Null_Equipment_Correctly()
    {
        // Act
        var result = _mapper.MapToDomain(null);

        // Assert
        Assert.That(result.GetEquipmentSlots(), Is.Not.Null);
        Assert.That(result.GetEquipmentSlots().Count, Is.EqualTo(0));
    }

    [Test]
    public void Handle_Empty_Equipment_Correctly()
    {
        // Arrange
        var equipment = new Equipment();

        // Act
        var result = _mapper.MapToDomain(equipment);

        // Assert
        Assert.That(result.GetEquipmentSlots(), Is.Not.Null);
        Assert.That(result.GetEquipmentSlots().Count, Is.EqualTo(0));
    }


    [Test]
    public void Log_Error_When_Equipment_Is_Null()
    {
        // Arrange
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        var loggerMock = new Mock<ILogger>();
        loggerFactoryMock.Setup(f => f.CreateLogger<BannerlordEquipmentMapper>()).Returns(loggerMock.Object);
        var mapper = new BannerlordEquipmentMapper(loggerFactoryMock.Object);

        // Act
        var result = mapper.MapToDomain(null);

        // Assert
        loggerMock.Verify(l => l.Error("Bannerlord equipment is null", null), Times.Once);
        Assert.That(result.GetEquipmentSlots(), Is.Not.Null);
        Assert.That(result.GetEquipmentSlots().Count, Is.EqualTo(0));
    }

    [Test]
    public void Map_Armor_Slot_With_Item_Should_Add_EquipmentSlot()
    {
        // Arrange
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        var loggerMock = new Mock<ILogger>();
        loggerFactoryMock.Setup(f => f.CreateLogger<BannerlordEquipmentMapper>()).Returns(loggerMock.Object);
        var mapper = new BannerlordEquipmentMapper(loggerFactoryMock.Object);

        var item = new ItemObject();
        var stringIdProp = typeof(ItemObject).GetProperty("StringId");
        stringIdProp?.SetValue(item, "test_item");

        var equipment = new Equipment();
        equipment[EquipmentIndex.Head] = new EquipmentElement(item);

        // Act
        var result = mapper.MapToDomain(equipment);

        // Assert
        var slots = result.GetEquipmentSlots();
        Assert.That(slots.Count, Is.EqualTo(1));
        var slot = slots.First();
        var expectedSlotId = Enum.GetName(typeof(EquipmentIndex), EquipmentIndex.Head);
        Assert.That(slot.SlotId, Is.EqualTo(expectedSlotId));
        Assert.That(slot.ItemId, Is.EqualTo("test_item"));
    }


    private static readonly IEnumerable<TestCaseData> ArmorSlotTestCases =
        Enum.GetValues(typeof(EquipmentIndex))
            .Cast<EquipmentIndex>()
            .Where(i => i >= EquipmentIndex.ArmorItemBeginSlot && i < EquipmentIndex.NumEquipmentSetSlots)
            .Select(i => new TestCaseData(i, $"item_{i}").SetName($"Map_{i}_Slot_With_Item_Should_Add_EquipmentSlot"));

    [Test]
    [TestCaseSource(nameof(ArmorSlotTestCases))]
    public void Map_Slot_With_Item_Should_Add_EquipmentSlot(EquipmentIndex index, string expectedItemId)
    {
        // Arrange
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        var loggerMock = new Mock<ILogger>();
        loggerFactoryMock.Setup(f => f.CreateLogger<BannerlordEquipmentMapper>()).Returns(loggerMock.Object);
        var mapper = new BannerlordEquipmentMapper(loggerFactoryMock.Object);

        var equipment = new Equipment();
        var stringIdProp = typeof(ItemObject).GetProperty("StringId");
        var item = new ItemObject();
        stringIdProp?.SetValue(item, expectedItemId);
        equipment[index] = new EquipmentElement(item);

        // Act
        var result = mapper.MapToDomain(equipment);

        // Assert
        var slots = result.GetEquipmentSlots();
        Assert.That(slots.Count, Is.EqualTo(1), "Expected exactly one equipment slot");
        var slot = slots.First();
        var expectedSlotId = Enum.GetName(typeof(EquipmentIndex), index);
        Assert.That(slot.SlotId, Is.EqualTo(expectedSlotId), "SlotId mismatch");
        Assert.That(slot.ItemId, Is.EqualTo(expectedItemId), "ItemId mismatch");
    }

    [Test]
    public void Map_Weapon_Slots()
    {
        // Arrange
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        var loggerMock = new Mock<ILogger>();
        loggerFactoryMock.Setup(f => f.CreateLogger<BannerlordEquipmentMapper>()).Returns(loggerMock.Object);
        var mapper = new BannerlordEquipmentMapper(loggerFactoryMock.Object);

        var equipment = new Equipment();
        var item = new ItemObject();
        var stringIdProp = typeof(ItemObject).GetProperty("StringId");
        stringIdProp?.SetValue(item, "test_weapon");
        equipment[EquipmentIndex.Weapon0] = new EquipmentElement(item);
        equipment[EquipmentIndex.Weapon1] = new EquipmentElement(item);
        equipment[EquipmentIndex.Weapon2] = new EquipmentElement(item);
        equipment[EquipmentIndex.Weapon3] = new EquipmentElement(item);
        equipment[EquipmentIndex.ExtraWeaponSlot] = new EquipmentElement(item);

        // Act
        var result = mapper.MapToDomain(equipment);

        // Assert
        var slots = result.GetEquipmentSlots();
        Assert.That(slots[0].SlotId, Is.EqualTo("Weapon0"));
        Assert.That(slots[1].SlotId, Is.EqualTo("Weapon1"));
        Assert.That(slots[2].SlotId, Is.EqualTo("Weapon2"));
        Assert.That(slots[3].SlotId, Is.EqualTo("Weapon3"));
    }
}
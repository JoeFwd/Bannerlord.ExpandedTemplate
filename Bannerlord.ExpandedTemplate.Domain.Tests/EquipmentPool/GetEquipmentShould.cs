using Bannerlord.ExpandedTemplate.Domain.EquipmentPool;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Util;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Domain.Tests.EquipmentPool;

public class GetEquipmentShould
{
    private Mock<IRandom> _random;

    private IGetEquipment _getEquipment;

    [SetUp]
    public void SetUp()
    {
        _random = new Mock<IRandom>(MockBehavior.Strict);
        _getEquipment = new GetEquipment(_random.Object);
    }

    [Test]
    public void GetFirstEquipmentWhenTwoEquipmentTemplatesAndRandomReturnsZero()
    {
        var equipment = new List<Equipment>
        {
            new(new List<EquipmentSlot> { new("item", "EquipmentId1") }),
            new(new List<EquipmentSlot> { new("item", "EquipmentId2") })
        };
        var equipmentPool = new Domain.EquipmentPool.Model.EquipmentPool(equipment, 0);
        _random.Setup(random => random.Next(0, 2)).Returns(0);

        var actualEquipment = _getEquipment.GetEquipmentFromEquipmentPool(equipmentPool);

        Assert.That(actualEquipment, Is.EqualTo(equipment[0]));
    }

    [Test]
    public void GetSecondEquipmentWhenTwoEquipmentTemplatesAndRandomReturnsOne()
    {
        var equipment = new List<Equipment>
        {
            new(new List<EquipmentSlot> { new("item", "EquipmentId1") }),
            new(new List<EquipmentSlot> { new("item", "EquipmentId2") })
        };
        var equipmentPool = new Domain.EquipmentPool.Model.EquipmentPool(equipment, 0);
        _random.Setup(random => random.Next(0, 2)).Returns(1);

        var actualEquipment = _getEquipment.GetEquipmentFromEquipmentPool(equipmentPool);

        Assert.That(actualEquipment, Is.EqualTo(equipment[1]));
    }

    [Test]
    public void ReturnsNullWhenNoEquipmentTemplates()
    {
        var equipmentPool = new Domain.EquipmentPool.Model.EquipmentPool(new List<Equipment>(), 0);

        var actualEquipment = _getEquipment.GetEquipmentFromEquipmentPool(equipmentPool);

        Assert.That(actualEquipment, Is.Null);
    }

    [Test]
    public void ReturnsNullWhenNoEquipmentPool()
    {
        var actualEquipment = _getEquipment.GetEquipmentFromEquipmentPool(null!);

        Assert.That(actualEquipment, Is.Null);
    }
}
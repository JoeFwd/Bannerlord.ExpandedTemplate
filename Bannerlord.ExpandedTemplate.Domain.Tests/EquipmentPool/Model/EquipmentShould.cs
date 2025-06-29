using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Domain.Tests.EquipmentPool.Model;

public class EquipmentShould
{
    [Test]
    public void EqualEquipmentWithSameEquipmentReference()
    {
        var equipment = CreateEquipment("1");

        var isEqual = equipment.Equals(equipment);

        Assert.That(isEqual, Is.True);
    }

    [Test]
    public void EqualEquipmentWithSameEquipmentContent()
    {
        var leftEquipment = CreateEquipment("1");
        var rightEquipment = CreateEquipment("1");

        var isEqual = leftEquipment.Equals(rightEquipment);

        Assert.That(isEqual, Is.True);
    }

    [Test]
    public void NotEqualEquipmentWithDifferentContent()
    {
        var leftEquipment = CreateEquipment("1");
        var rightEquipment = CreateEquipment("2");

        var isEqual = leftEquipment.Equals(rightEquipment);

        Assert.That(isEqual, Is.False);
    }

    private Equipment CreateEquipment(string id)
    {
        return new Equipment(new List<EquipmentSlot> { new("item", id) });
    }
}
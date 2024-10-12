using System.Xml.Linq;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Domain.Tests.EquipmentPool.Model;

public class EquipmentShould
{
    [Test]
    public void EqualEquipmentWithSameEquipmentReference()
    {
        var equipment = new Equipment(CreateEquipmentNode("1"));

        var isEqual = equipment.Equals(equipment);

        Assert.That(isEqual, Is.True);
    }

    [Test]
    public void EqualEquipmentWithSameEquipmentContent()
    {
        var leftEquipment = new Equipment(CreateEquipmentNode("1"));
        var rightEquipment = new Equipment(CreateEquipmentNode("1"));

        var isEqual = leftEquipment.Equals(rightEquipment);

        Assert.That(isEqual, Is.True);
    }

    [Test]
    public void NotEqualEquipmentWithDifferentContent()
    {
        var leftEquipment = new Equipment(CreateEquipmentNode("1"));
        var rightEquipment = new Equipment(CreateEquipmentNode("2"));

        var isEqual = leftEquipment.Equals(rightEquipment);

        Assert.That(isEqual, Is.False);
    }

    private XNode CreateEquipmentNode(string id)
    {
        return XDocument.Parse($"<EquipmentRoster id=\"EquipmentLoadout{id}\"/>").Root!;
    }
}
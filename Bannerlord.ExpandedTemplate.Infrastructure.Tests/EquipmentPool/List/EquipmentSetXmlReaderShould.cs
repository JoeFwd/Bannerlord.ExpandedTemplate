using System.Xml.Linq;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Xml;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List;

public class EquipmentSetXmlReaderShould
{
    private IEquipmentSetXmlReader _reader;

    [SetUp]
    public void SetUp()
    {
        _reader = new EquipmentSetXmlReader();
    }

    [Test]
    public void Handle_Lowercase_Equipment_Tag()
    {
        // Arrange: create an XML document with a proper <EquipmentSet> root but a lower‑case <equipment> element.
        var xml = new XDocument(
            new XElement("EquipmentSet",
                new XAttribute("pool", "some_pool"),
                new XElement("equipment", // intentionally lower‑case
                    new XAttribute("slot", "slot1"),
                    new XAttribute("id", "123")
                )
            )
        );

        // Act: read the equipment set.
        string xmlString = xml.ToString();
        EquipmentSet? result = _reader.Read(xmlString);

        // Assert: the result should not be null and should contain the parsed equipment.
        Assert.IsNotNull(result, "Reader should return a non‑null EquipmentSet for a valid root.");
        Assert.That(result!.Equipment.Count, Is.EqualTo(1), "Expected exactly one equipment entry.");
        Assert.That(result.Equipment[0].Slot, Is.EqualTo("slot1"), "Equipment slot should be parsed correctly.");
        Assert.That(result.Equipment[0].Id, Is.EqualTo("123"), "Equipment id should be parsed correctly.");
    }

    [Test]
    public void Handle_Uppercase_Equipment_Tag()
    {
        // Arrange: create an XML document with a proper <EquipmentSet> root but a lower‑case <equipment> element.
        var xml = new XDocument(
            new XElement("EquipmentSet",
                new XAttribute("pool", "some_pool"),
                new XElement("Equipment", // intentionally lower‑case
                    new XAttribute("slot", "slot1"),
                    new XAttribute("id", "123")
                )
            )
        );

        // Act: read the equipment set.
        string xmlString = xml.ToString();
        EquipmentSet? result = _reader.Read(xmlString);

        // Assert: the result should not be null and should contain the parsed equipment.
        Assert.IsNotNull(result, "Reader should return a non‑null EquipmentSet for a valid root.");
        Assert.That(result!.Equipment.Count, Is.EqualTo(1), "Expected exactly one equipment entry.");
        Assert.That(result.Equipment[0].Slot, Is.EqualTo("slot1"), "Equipment slot should be parsed correctly.");
        Assert.That(result.Equipment[0].Id, Is.EqualTo("123"), "Equipment id should be parsed correctly.");
    }

    [Test]
    public void Handle_Lowercase_Root_Element()
    {
        var xml = new XDocument(
            new XElement("equipmentset", // lower‑case root
                new XAttribute("pool", "some_pool"),
                new XElement("Equipment",
                    new XAttribute("slot", "slot1"),
                    new XAttribute("id", "123")
                )
            )
        );
        var result = _reader.Read(xml.ToString());
        Assert.IsNotNull(result, "Reader should return a non‑null EquipmentSet for a lower‑case root element.");
        Assert.That(result!.Equipment.Count, Is.EqualTo(1), "Expected exactly one equipment entry.");
    }

    [Test]
    public void Handle_Uppercase_Root_Element()
    {
        var xml = new XDocument(
            new XElement("EQUIPMENTSET", // upper‑case root
                new XAttribute("pool", "some_pool"),
                new XElement("Equipment",
                    new XAttribute("slot", "slot1"),
                    new XAttribute("id", "123")
                )
            )
        );
        var result = _reader.Read(xml.ToString());
        Assert.IsNotNull(result, "Reader should return a non‑null EquipmentSet for an upper‑case root element.");
        Assert.That(result!.Equipment.Count, Is.EqualTo(1), "Expected exactly one equipment entry.");
    }
}
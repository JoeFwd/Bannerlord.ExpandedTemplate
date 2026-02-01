using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Xml;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List;

[TestFixture]
public class EquipmentRosterXmlReaderShould
{
    private IEquipmentRosterXmlReader _reader = null!;

    // No longer needed: private Moq.Mock<IEquipmentSetXmlReader> _setReaderMock = null!;
    private Mock<IEquipmentSetXmlReader> _setReaderMock = null!;

    [SetUp]
    public void SetUp()
    {
        // Create a mock IEquipmentSetXmlReader that returns a minimal EquipmentSet
        _setReaderMock = new Mock<IEquipmentSetXmlReader>();
        _setReaderMock.Setup(m => m.Read(It.IsAny<string>()))
            .Returns(new EquipmentSet { Equipment = new List<Equipment>() });
        _reader = new EquipmentRosterXmlReader(_setReaderMock.Object);
    }

    [Test]
    public void ReadEquipmentRosterWithSingleSet()
    {
        // Arrange
        var xml =
            "<EquipmentRoster>\n    <EquipmentSet>\n        <Equipment id=\"1\">Sword</Equipment>\n    </EquipmentSet>\n</EquipmentRoster>";

        // Act
        var result = _reader.Read(xml);

        // Assert
        Assert.IsNotNull(result, "EquipmentRoster should not be null.");
        Assert.That(result!.EquipmentSet.Count, Is.EqualTo(1), "There should be exactly one EquipmentSet.");
    }

    [Test]
    public void ReadId()
    {
        // Arrange
        var xml =
            "<EquipmentRoster id=\"1\">\n    <EquipmentSet>\n        <Equipment id=\"1\">Sword</Equipment>\n    </EquipmentSet>\n</EquipmentRoster>";

        // Act
        var result = _reader.Read(xml);

        // Assert
        Assert.IsNotNull(result, "EquipmentRoster should not be null.");
        Assert.That(result.Id, Is.EqualTo("1"));
    }
}
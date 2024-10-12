using System.Xml.Serialization;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;

[XmlRoot(ElementName = "Equipment")]
public record Equipment
{
    [XmlAttribute(AttributeName = "slot")] public string? Slot { get; init; }
    [XmlAttribute(AttributeName = "id")] public string? Id { get; init; }
}
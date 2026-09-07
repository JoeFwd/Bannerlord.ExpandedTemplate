using System.Xml.Serialization;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

[XmlRoot(ElementName = "EquipmentSet")]
public record EquipmentSet
{
    public EquipmentSet()
    {
    }

    public EquipmentSet(EquipmentSet equipmentSet)
    {
        IsBattle = equipmentSet.IsBattle;
        Id = equipmentSet.Id;
        IsCivilian = equipmentSet.IsCivilian;
        IsSiege = equipmentSet.IsSiege;
        Pool = equipmentSet.Pool;
        EquipmentType = equipmentSet.EquipmentType;
        EquipmentTypes = equipmentSet.EquipmentTypes;
    }

    [XmlAttribute(AttributeName = "id")] public string? Id { get; init; }

    [XmlAttribute(AttributeName = "battle")]
    public string? IsBattle { get; init; }

    [XmlAttribute(AttributeName = "civilian")]
    public string? IsCivilian { get; init; }

    [XmlAttribute(AttributeName = "siege")]
    public string? IsSiege { get; init; }

    [XmlAttribute(AttributeName = "pool")] public string? Pool { get; init; }

    [XmlAttribute(AttributeName = "equipmentType")]
    public string? EquipmentType { get; init; }

    [XmlAttribute(AttributeName = "equipmentTypes")]
    public string? EquipmentTypes { get; init; }

    [XmlIgnore]
    public string? ResolvedIsBattle => EquipmentTypeFlags.Resolve(IsBattle, EquipmentType, EquipmentTypes, EquipmentTypeFlags.Battle);

    [XmlIgnore]
    public string? ResolvedIsSiege => EquipmentTypeFlags.Resolve(IsSiege, EquipmentType, EquipmentTypes, EquipmentTypeFlags.Siege);

    [XmlIgnore]
    public string? ResolvedIsCivilian =>
        EquipmentTypeFlags.Resolve(IsCivilian, EquipmentType, EquipmentTypes, EquipmentTypeFlags.Civilian);

    [XmlIgnore]
    public string? ResolvedIsStealth =>
        EquipmentTypeFlags.Resolve(null, EquipmentType, EquipmentTypes, EquipmentTypeFlags.Stealth);
}
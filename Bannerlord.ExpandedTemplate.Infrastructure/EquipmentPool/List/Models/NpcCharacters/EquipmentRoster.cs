using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

[XmlRoot(ElementName = "EquipmentRoster")]
public record EquipmentRoster
{
    public EquipmentRoster()
    {
    }

    public EquipmentRoster(EquipmentRoster equipmentRoster)
    {
        IsBattle = equipmentRoster.IsBattle;
        IsCivilian = equipmentRoster.IsCivilian;
        IsSiege = equipmentRoster.IsSiege;
        Pool = equipmentRoster.Pool;
        Equipment = equipmentRoster.Equipment.Select(equipment => new Equipment(equipment)).ToList();
    }

    [XmlElement(ElementName = "equipment")]
    public List<Equipment> Equipment { get; init; } = new();

    [XmlAttribute(AttributeName = "battle")]
    public string? IsBattle { get; init; }
    
    [XmlAttribute(AttributeName = "civilian")]
    public string? IsCivilian { get; init; }

    [XmlAttribute(AttributeName = "siege")]
    public string? IsSiege { get; init; }

    [XmlAttribute(AttributeName = "pool")] public string? Pool { get; init; }

    public virtual bool Equals(EquipmentRoster? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equipment.SequenceEqual(other.Equipment) && IsBattle == other.IsBattle &&
               IsCivilian == other.IsCivilian &&
               IsSiege == other.IsSiege && Pool == other.Pool;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = 17;
            hashCode = (hashCode * 397) ^ (IsBattle != null ? IsBattle.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (IsCivilian != null ? IsCivilian.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (IsSiege != null ? IsSiege.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Pool != null ? Pool.GetHashCode() : 0);

            foreach (var equipment in Equipment) hashCode = hashCode * 31 + equipment.GetHashCode();

            return hashCode;
        }
    }
}
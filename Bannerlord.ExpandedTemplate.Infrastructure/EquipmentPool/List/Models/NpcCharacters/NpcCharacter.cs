using System.Xml.Serialization;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

[XmlRoot(ElementName = "NPCCharacter")]
public record NpcCharacter
{
    public NpcCharacter()
    {
    }

    public NpcCharacter(NpcCharacter npcCharacter)
    {
        Id = npcCharacter.Id;
        Equipments = new Equipments(npcCharacter.Equipments);
    }

    [XmlElement(ElementName = "Equipments")]
    public Equipments Equipments { get; init; } = new();

    [XmlAttribute(AttributeName = "id")] public string? Id { get; init; }

    public virtual bool Equals(NpcCharacter? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equipments.Equals(other.Equipments) && Id == other.Id;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Equipments.GetHashCode() * 397) ^ (Id != null ? Id.GetHashCode() : 0);
        }
    }
}
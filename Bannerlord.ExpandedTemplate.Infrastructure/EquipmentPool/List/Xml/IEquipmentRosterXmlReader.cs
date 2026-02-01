using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Xml;

/// <summary>
///     Contract for reading an EquipmentRoster from its XML representation.
/// </summary>
public interface IEquipmentRosterXmlReader
{
    /// <summary>
    ///     Parses the supplied XML and returns the corresponding <see cref="EquipmentRoster" />.
    ///     Returns null if the XML cannot be parsed (stub implementation).
    /// </summary>
    EquipmentRoster? Read(string xml);
}
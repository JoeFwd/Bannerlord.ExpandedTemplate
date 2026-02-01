using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Xml;

/// <summary>
///     Reads an <see cref="EquipmentSet" /> from an XML element.
/// </summary>
public interface IEquipmentSetXmlReader
{
    /// <summary>
    ///     Reads the equipment set from the supplied XML element.
    /// </summary>
    /// <param name="element">The XML element representing the equipment set.</param>
    /// <returns>The parsed <see cref="EquipmentSet" /> or <c>null</c> if the element is invalid.</returns>
    EquipmentSet? Read(string xml);
}
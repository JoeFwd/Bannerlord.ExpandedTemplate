using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Xml;

/// <summary>
///     Abstraction for reading equipment rosters from an XML source.
/// </summary>
public interface IEquipmentRostersReader
{
    /// <summary>
    ///     Parses the provided XML and returns every <c>EquipmentRoster</c> found.
    /// </summary>
    /// <param name="xml">The XML string containing an <c>EquipmentRosters</c> root element.</param>
    /// <returns>An enumerable of <see cref="EquipmentRoster" /> instances.</returns>
    IEnumerable<EquipmentRoster> ReadAll(string xml);
}
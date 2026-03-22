using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Xml;

/// <summary>
///     Reads all <see cref="EquipmentRoster" /> elements from an XML document.
/// </summary>
public class EquipmentRostersReader : IEquipmentRostersReader
{
    private readonly IEquipmentRosterXmlReader _rosterReader;

    public EquipmentRostersReader(IEquipmentRosterXmlReader rosterReader)
    {
        _rosterReader = rosterReader ?? throw new ArgumentNullException(nameof(rosterReader));
    }

    /// <summary>
    ///     Parses the provided XML and returns every <c>EquipmentRoster</c> found.
    /// </summary>
    /// <param name="xml">The XML string containing an <c>EquipmentRosters</c> root element.</param>
    /// <returns>An enumerable of <see cref="EquipmentRoster" /> instances.</returns>
    public IEnumerable<EquipmentRoster> ReadAll(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return Enumerable.Empty<EquipmentRoster>();

        var doc = XDocument.Parse(xml);
        var rosterElements = doc
            .Descendants()
            .Where(e => e.Name.LocalName.Equals("EquipmentRoster", StringComparison.OrdinalIgnoreCase));

        var rosters = rosterElements
            .Select(e => _rosterReader.Read(e.ToString()))
            .Where(r => r != null)
            .Select(r => r!);

        return rosters;
    }
}
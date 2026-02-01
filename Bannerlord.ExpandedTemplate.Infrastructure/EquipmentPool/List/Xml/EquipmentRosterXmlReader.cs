using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Xml;

/// <summary>
///     Reads an <see cref="EquipmentRoster" /> from its XML representation.
/// </summary>
public class EquipmentRosterXmlReader : IEquipmentRosterXmlReader
{
    private readonly IEquipmentSetXmlReader _equipmentSetXmlReader;

    public EquipmentRosterXmlReader() : this(new EquipmentSetXmlReader())
    {
    }

    public EquipmentRosterXmlReader(IEquipmentSetXmlReader equipmentSetXmlReader) =>
        _equipmentSetXmlReader = equipmentSetXmlReader ?? throw new ArgumentNullException(nameof(equipmentSetXmlReader));

    private static XElement GetRootElement(string xml)
    {
        var doc = XDocument.Parse(xml);
        var root = doc.Root ?? throw new ArgumentException("XML does not contain a root element.", nameof(xml));

        var rosterElement = root.Name.LocalName.Equals("EquipmentRoster", StringComparison.OrdinalIgnoreCase)
            ? root
            : root.Descendants().FirstOrDefault(e => e.Name.LocalName.Equals("EquipmentRoster", StringComparison.OrdinalIgnoreCase));

        return rosterElement ?? throw new ArgumentException("EquipmentRoster element not found in XML.", nameof(xml));
    }

    private static IEnumerable<XElement> GetEquipmentSetElements(XElement root) =>
        root.Elements().Where(e => e.Name.LocalName.Equals("EquipmentSet", StringComparison.OrdinalIgnoreCase));

    public EquipmentRoster? Read(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return null;

        var rosterElement = GetRootElement(xml);

        var sets = GetEquipmentSetElements(rosterElement)
            .Select(e => _equipmentSetXmlReader.Read(e.ToString()))
            .Where(s => s != null)
            .Select(s => s!)
            .ToList();

        return new EquipmentRoster
        {
            Id = rosterElement.Attribute("id")?.Value,
            EquipmentSet = sets
        };
    }
}

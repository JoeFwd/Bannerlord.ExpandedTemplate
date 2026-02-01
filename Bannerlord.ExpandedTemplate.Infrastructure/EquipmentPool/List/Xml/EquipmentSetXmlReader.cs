using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Xml;

/// <summary>
///     Reads an <see cref="EquipmentSet" /> from its XML representation.
///     Returns <c>null</c> when the root element name is not exactly "EquipmentSet" (case‑sensitive).
///     Handles both <c>&lt;Equipment&gt;</c> and lower‑case <c>&lt;equipment&gt;</c> elements.
/// </summary>
public class EquipmentSetXmlReader : IEquipmentSetXmlReader
{
    public EquipmentSet? Read(string xml)
    {
        var doc = XDocument.Parse(xml);
        var root = doc.Root;
        if (root == null) return null;

        if (!string.Equals(root.Name.LocalName, "EquipmentSet", StringComparison.OrdinalIgnoreCase))
            return null;

        var equipmentElements = GetEquipmentElements(root);

        var equipments = equipmentElements.Select(e => new Equipment
        {
            Slot = e.Attribute("slot")?.Value,
            Id = e.Attribute("id")?.Value
        }).ToList();

        return new EquipmentSet
        {
            Equipment = equipments,
            IsBattle = root.Attribute("battle")?.Value,
            IsCivilian = root.Attribute("civilian")?.Value,
            IsSiege = root.Attribute("siege")?.Value,
            Pool = root.Attribute("pool")?.Value
        };
    }

    private IEnumerable<XElement> GetEquipmentElements(XElement root)
    {
        return root.Elements()
            .Where(e => string.Equals(e.Name.LocalName, "Equipment", StringComparison.OrdinalIgnoreCase));
    }
}
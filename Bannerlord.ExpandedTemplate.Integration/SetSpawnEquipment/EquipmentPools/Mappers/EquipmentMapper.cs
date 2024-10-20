using System;
using System.Xml;
using System.Xml.Linq;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.EquipmentPools.Mappers;

public class EquipmentMapper(MBObjectManager mbObjectManager, ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<EquipmentMapper>();

    public Equipment Map(Domain.EquipmentPool.Model.Equipment equipment, MBEquipmentRoster bannerlordEquipmentPool)
    {
        XmlNode? xmlEquipmentNode = MapEquipmentNode(equipment.GetEquipmentNode());
        if (xmlEquipmentNode is null) return null;

        if (xmlEquipmentNode.Name.Equals("EquipmentRoster", StringComparison.InvariantCultureIgnoreCase))
            return AddEquipmentNodeToEquipmentRoster(xmlEquipmentNode, bannerlordEquipmentPool);

        return null;
    }

    private XmlNode? MapEquipmentNode(XNode node)
    {
        if (node is null) return null;
        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(node.ToString());
        return xmlDocument.DocumentElement;
    }

    private Equipment AddEquipmentNodeToEquipmentRoster(XmlNode equipmentRosterNode,
        MBEquipmentRoster bannerlordEquipmentPool)
    {
        var equipmentLoadout =
            new Equipment(bool.Parse(equipmentRosterNode.Attributes?["civilian"]?.Value ?? "false"));
        equipmentLoadout.Deserialize(mbObjectManager, equipmentRosterNode);

        var nativeEquipmentLoadout =
            FindMatchingDomainEquipmentInBannerlordEquipmentPool(bannerlordEquipmentPool, equipmentLoadout);

        if (nativeEquipmentLoadout is null)
        {
            _logger.Error(
                $"Could not find {equipmentLoadout} among native '{bannerlordEquipmentPool.StringId}' equipment roster");
            return null;
        }

        return nativeEquipmentLoadout;
    }

    private Equipment? FindMatchingDomainEquipmentInBannerlordEquipmentPool(MBEquipmentRoster bannerlordEquipmentPool,
        Equipment equipment)
    {
        if (bannerlordEquipmentPool is null) return null;

        return bannerlordEquipmentPool.AllEquipments.Find(nativeEquipmentLoadout =>
            nativeEquipmentLoadout.IsEquipmentEqualTo(equipment));
    }
}
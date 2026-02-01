using System.IO;
using System.Linq;
using System.Xml.Linq;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Xml;
using Bannerlord.ExpandedTemplate.Infrastructure.Exception;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories
{
    public class EquipmentRosterRepository : IEquipmentRosterRepository
    {
        internal const string EquipmentRostersRootTag = "EquipmentRosters";
        internal const string DeserialisationErrorMessage = "Error while trying to deserialise equipment rosters";
        internal const string IoErrorMessage = "Error while trying to get equipment rosters";

        private readonly ILogger _logger;
        private readonly IXmlProcessor _xmlProcessor;
        private readonly IEquipmentRostersReader _rostersReader;
        private readonly ICachingProvider _cachingProvider;
        private string? _cachedObjectId;
        private EquipmentRosters? _cachedEquipmentRosters;

        public EquipmentRosterRepository(
            IXmlProcessor xmlProcessor,
            IEquipmentRostersReader rostersReader,
            ICachingProvider cachingProvider,
            ILoggerFactory loggerFactory)
        {
            _xmlProcessor = xmlProcessor;
            _rostersReader = rostersReader;
            _cachingProvider = cachingProvider;
            _logger = loggerFactory.CreateLogger<IEquipmentRosterRepository>();
        }

        public EquipmentRosters GetEquipmentRosters()
        {
            if (!string.IsNullOrEmpty(_cachedObjectId))
            {
                _cachedEquipmentRosters = _cachingProvider.GetObject<EquipmentRosters>(_cachedObjectId);
            }

            if (_cachedEquipmentRosters != null)
                return _cachedEquipmentRosters;

            try
            {
                var xmlNode = _xmlProcessor.GetXmlNodes(EquipmentRostersRootTag);
                if (xmlNode == null)
                    throw new TechnicalException(DeserialisationErrorMessage);

                if (xmlNode is not XDocument doc || doc.Root == null || doc.Root.Name != EquipmentRostersRootTag)
                    throw new TechnicalException(DeserialisationErrorMessage);

                var rosters = _rostersReader.ReadAll(xmlNode.ToString()).ToList();
                var equipmentRosters = new EquipmentRosters { EquipmentRoster = rosters };
                CacheEquipmentRosters(equipmentRosters);
                return equipmentRosters;
            }
            catch (IOException)
            {
                throw new TechnicalException(IoErrorMessage);
            }
            catch (System.Exception e)
            {
                throw new TechnicalException(DeserialisationErrorMessage, e);
            }
        }

        private void CacheEquipmentRosters(EquipmentRosters equipmentRosters)
        {
            _cachedObjectId = _cachingProvider.CacheObject(equipmentRosters, CacheDataType.Xml);
            _cachedEquipmentRosters = equipmentRosters;
        }
    }
}

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.Exception;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories
{
    public class EquipmentRosterRepository(
        IXmlProcessor xmlProcessor,
        ICachingProvider cachingProvider,
        ILoggerFactory loggerFactory) : IEquipmentRosterRepository
    {
        internal const string EquipmentRostersRootTag = "EquipmentRosters";
        internal const string DeserialisationErrorMessage = "Error while trying to deserialise equipment rosters";
        internal const string IoErrorMessage = "Error while trying to get equipment rosters";

        private readonly ILogger _logger = loggerFactory.CreateLogger<IEquipmentRosterRepository>();
        private string? _cachedObjectId;

        public EquipmentRosters GetEquipmentRosters()
        {
            if (_cachedObjectId is not null)
            {
                EquipmentRosters? cachedNpcCharacters =
                    cachingProvider.GetObject<EquipmentRosters>(_cachedObjectId);
                if (cachedNpcCharacters is not null) return cachedNpcCharacters;

                _logger.Error("The cached equipment rosters are null.");
                return new EquipmentRosters();
            }
            
            try
            {
                using XmlReader xmlReader = xmlProcessor.GetXmlNodes(EquipmentRostersRootTag).CreateReader();

                var serialiser = new XmlSerializer(typeof(EquipmentRosters));
                EquipmentRosters equipmentRosters = (EquipmentRosters)serialiser.Deserialize(xmlReader);

                CacheNpcCharacters(equipmentRosters);

                return equipmentRosters;
            }
            catch (IOException e)
            {
                throw new TechnicalException(IoErrorMessage, e);
            }
            catch (InvalidOperationException e)
            {
                throw new TechnicalException(DeserialisationErrorMessage, e.GetBaseException());
            }
        }

        private void CacheNpcCharacters(EquipmentRosters equipmentRosters)
        {
            _cachedObjectId = cachingProvider.CacheObject(equipmentRosters, CacheDataType.Xml);
        }
    }
}
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.ExpandedTemplate.Integration.EquipmentPool.List.Repositories.Spi
{
    public class MergedModulesXmlProcessor : IXmlProcessor
    {
        private readonly ILogger _logger;
        private readonly ICachingProvider _cachingProvider;
        private readonly Dictionary<string, string> _cachedXmlDocumentKeys = new();

        public MergedModulesXmlProcessor(ILoggerFactory loggerFactory, ICachingProvider cachingProvider)
        {
            _logger = loggerFactory.CreateLogger<MergedModulesXmlProcessor>();
            _cachingProvider = cachingProvider;
        }

        public XNode GetXmlNodes(string rootElementName)
        {
            if (_cachedXmlDocumentKeys.TryGetValue(rootElementName, out var xmlDocumentKey))
            {
                var cachedXmlDocument = _cachingProvider.GetObject<XDocument>(xmlDocumentKey);
                if (cachedXmlDocument is not null) return cachedXmlDocument;
            }

            var xmlDocument = GetMergedXmlCharacterNodes(rootElementName);

            _cachedXmlDocumentKeys[rootElementName] =
                _cachingProvider.CacheObject(xmlDocument, CacheDataType.Xml);

            return xmlDocument;
        }

        private XDocument GetMergedXmlCharacterNodes(string rootTag)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var characterDocument = MBObjectManager.GetMergedXmlForManaged(rootTag, true);
                sw.Stop();

                _logger.Debug($"GetMergedXmlForManaged took: {sw.ElapsedMilliseconds}ms");

                return XDocument.Parse(characterDocument.OuterXml);
            }
            catch (IOException e)
            {
                _logger.Error($"Failed to get merged Xml character nodes: {e}");
                return new XDocument();
            }
        }
    }
}
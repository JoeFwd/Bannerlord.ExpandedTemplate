using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.Exception;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories;

public class NpcCharacterRepository(
    IXmlProcessor xmlProcessor,
    ICachingProvider cachingProvider,
    ILoggerFactory loggerFactory) : INpcCharacterRepository
{
    internal const string NpcCharacterRootTag = "NPCCharacters";
    internal const string DeserialisationErrorMessage = "Error while trying to deserialise npc characters";
    internal const string IoErrorMessage = "Error while trying to get npc characters";

    private readonly ILogger _logger = loggerFactory.CreateLogger<INpcCharacterRepository>();
    private string? _cachedObjectId;

    public NpcCharacters GetNpcCharacters()
    {
        try
        {
            if (_cachedObjectId is not null)
            {
                NpcCharacters? cachedNpcCharacters =
                    cachingProvider.GetObject<NpcCharacters>(_cachedObjectId);
                if (cachedNpcCharacters is not null) return cachedNpcCharacters;

                _logger.Error("The cached npc characters are null.");
                return new NpcCharacters();
            }
                
            using XmlReader xmlReader = xmlProcessor.GetXmlNodes(NpcCharacterRootTag).CreateReader();

            var serialiser = new XmlSerializer(typeof(NpcCharacters));
            NpcCharacters npcCharacters = (NpcCharacters)serialiser.Deserialize(xmlReader);

            npcCharacters = MergeDuplicateCharacters(npcCharacters);

            CacheNpcCharacters(npcCharacters);

            return npcCharacters;
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

    private NpcCharacters MergeDuplicateCharacters(NpcCharacters npcCharacters)
    {
        var characterGroups = npcCharacters.NpcCharacter.GroupBy(c => c.Id);

        var mergedCharacters = characterGroups.Select(group =>
        {
            var firstCharacter = group.First();
            var mergedEquipments = new Equipments
            {
                EquipmentRoster = group.SelectMany(c => c.Equipments.EquipmentRoster).ToList(),
                EquipmentSet = group.SelectMany(c => c.Equipments.EquipmentSet).ToList()
            };

            return new NpcCharacter
            {
                Id = firstCharacter.Id,
                Equipments = mergedEquipments
            };
        }).ToList();

        return new NpcCharacters { NpcCharacter = mergedCharacters };
    }

    private void CacheNpcCharacters(NpcCharacters npcCharacters)
    {
        _cachedObjectId = cachingProvider.CacheObject(npcCharacters, CacheDataType.Xml);
    }
}
using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters;

public class NpcCharacterWithResolvedEquipmentProvider : INpcCharacterWithResolvedEquipmentProvider
{
    private readonly INpcCharacterMapper _npcCharacterMapper;
    private readonly INpcCharacterRepository _npcCharacterRepository;

    public NpcCharacterWithResolvedEquipmentProvider(
        INpcCharacterRepository npcCharacterRepository,
        INpcCharacterMapper npcCharacterMapper,
        ILoggerFactory loggerFactory)
    {
        _npcCharacterRepository = npcCharacterRepository;
        _npcCharacterMapper = npcCharacterMapper;
    }

    public IDictionary<string, IList<EquipmentRoster>> GetNpcCharactersWithResolvedEquipmentRoster()
    {
        IDictionary<string, IList<EquipmentRoster>> equipmentRostersByCharacterId = _npcCharacterRepository
            .GetNpcCharacters().NpcCharacter
            .Where(character => character.Id is not null)
            .ToDictionary(character => character.Id!, character => _npcCharacterMapper
                .MapToEquipmentRosters(character));

        return equipmentRostersByCharacterId;
    }
}
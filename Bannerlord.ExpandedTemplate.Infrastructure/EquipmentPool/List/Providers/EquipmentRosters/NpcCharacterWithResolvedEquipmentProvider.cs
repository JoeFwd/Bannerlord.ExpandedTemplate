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
    private readonly ILogger _logger;
    private readonly ICacheProvider _cacheProvider;
    private readonly INpcCharacterMapper _npcCharacterMapper;
    private readonly INpcCharacterRepository _npcCharacterRepository;

    private string? _onSessionLaunchedCachedObjectId;

    public NpcCharacterWithResolvedEquipmentProvider(
        ICacheProvider cacheProvider,
        INpcCharacterRepository npcCharacterRepository,
        INpcCharacterMapper npcCharacterMapper,
        ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<NpcCharacterWithResolvedEquipmentProvider>();
        _cacheProvider = cacheProvider;
        _npcCharacterRepository = npcCharacterRepository;
        _npcCharacterMapper = npcCharacterMapper;
    }

    public IDictionary<string, IList<EquipmentRoster>> GetNpcCharactersWithResolvedEquipmentRoster()
    {
        if (_onSessionLaunchedCachedObjectId is not null)
        {
            IDictionary<string, IList<EquipmentRoster>>? cachedNpcCharacters =
                _cacheProvider
                    .GetCachedObject<IDictionary<string, IList<EquipmentRoster>>>(
                        _onSessionLaunchedCachedObjectId);
            if (cachedNpcCharacters is not null) return cachedNpcCharacters;

            _logger.Error("The cached equipment pools are null.");
            return new Dictionary<string, IList<EquipmentRoster>>();
        }

        IDictionary<string, IList<EquipmentRoster>> equipmentRostersByCharacterId = _npcCharacterRepository
            .GetNpcCharacters().NpcCharacter
            .Where(character => character.Id is not null)
            .ToDictionary(character => character.Id!, character => _npcCharacterMapper
                .MapToEquipmentRosters(character));

        _onSessionLaunchedCachedObjectId =
            _cacheProvider.CacheObject(equipmentRostersByCharacterId);
        _cacheProvider.InvalidateCache(_onSessionLaunchedCachedObjectId, CampaignEvent.OnAfterSessionLaunched);

        return equipmentRostersByCharacterId;
    }
}
using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Pool;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;

public class EquipmentPoolsProvider : IEquipmentPoolsProvider
{
    private readonly IEquipmentRostersProvider _equipmentRostersProvider;
    private readonly IPoolEquipmentRosterProvider _poolEquipmentRosterProvider;
    private readonly IEquipmentRosterMapper _equipmentRosterMapper;
    private readonly ICachingProvider _cachingProvider;

    private string? _cacheId;

    public EquipmentPoolsProvider(
        IEquipmentRostersProvider equipmentRostersProvider,
        IPoolEquipmentRosterProvider poolEquipmentRosterProvider,
        IEquipmentRosterMapper equipmentRosterMapper,
        ICachingProvider cachingProvider)
    {
        _equipmentRostersProvider = equipmentRostersProvider;
        _poolEquipmentRosterProvider = poolEquipmentRosterProvider;
        _equipmentRosterMapper = equipmentRosterMapper;
        _cachingProvider = cachingProvider;
    }

    public IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>> GetEquipmentPoolsByCharacterId()
    {
        if (_cacheId is not null)
        {
            var cachedEquipmentRostersByPoolAndCharacter = _cachingProvider
                .GetObject<IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>>(_cacheId);

            if (cachedEquipmentRostersByPoolAndCharacter is not null) return cachedEquipmentRostersByPoolAndCharacter;
        }

        var equipmentRostersByCharacter = _equipmentRostersProvider.GetEquipmentRostersByCharacter();
        var equipmentRostersByPoolAndCharacter = _poolEquipmentRosterProvider.GetEquipmentRostersByPoolAndCharacter();

        var result = equipmentRostersByPoolAndCharacter.ToDictionary(
            pair => pair.Key,
            pair => MapToEquipmentPools(pair.Key, pair.Value, equipmentRostersByCharacter)
        );

        _cacheId = _cachingProvider.CacheObject(result, CacheDataType.EquipmentPools);

        return result;
    }

    private IList<Domain.EquipmentPool.Model.EquipmentPool> MapToEquipmentPools(
        string npcCharacterId,
        IDictionary<string, IList<EquipmentRoster>> characterEquipmentPools,
        IDictionary<string, IList<EquipmentRoster>> allEquipmentRostersByCharacter)
    {
        if (!allEquipmentRostersByCharacter.TryGetValue(npcCharacterId, out var npcRosters))
            return new List<Domain.EquipmentPool.Model.EquipmentPool>();

        var matchedRosters = FilterMatchingRosters(characterEquipmentPools, npcRosters);
        return matchedRosters
            .Select(pool => MapToEquipmentPool(pool.Key, pool.Value))
            .Where(pool => !pool.IsEmpty())
            .ToList();
    }

    private static IDictionary<string, IList<EquipmentRoster>> FilterMatchingRosters(
        IDictionary<string, IList<EquipmentRoster>> rostersByPool,
        IList<EquipmentRoster> rostersToKeep)
    {
        return rostersByPool.ToDictionary(
            pool => pool.Key,
            pool => pool.Value.Intersect(rostersToKeep).ToList() as IList<EquipmentRoster>);
    }


    private Domain.EquipmentPool.Model.EquipmentPool MapToEquipmentPool(
        string poolKey, IList<EquipmentRoster> equipmentRosters)
    {
        var combinedEquipment = equipmentRosters
            .SelectMany(roster => _equipmentRosterMapper.MapToEquipmentPool(roster).GetEquipmentLoadouts())
            .ToList();

        return new Domain.EquipmentPool.Model.EquipmentPool(
            combinedEquipment,
            ParsePoolKey(poolKey)
        );
    }

    private static int ParsePoolKey(string poolKey)
    {
        return int.TryParse(poolKey, out var pool) ? pool : 0;
    }
}
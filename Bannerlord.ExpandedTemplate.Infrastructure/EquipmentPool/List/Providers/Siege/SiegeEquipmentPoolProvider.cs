﻿using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentSorters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.Siege
{
    public class SiegeEquipmentPoolProvider(ILoggerFactory loggerFactory,
        ICacheProvider cacheProvider,
        params IEquipmentPoolsRepository[] equipmentRepositories) : ISiegeEquipmentPoolProvider
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<SiegeEquipmentPoolProvider>();
        private string? _onSessionLaunchedCachedObjectId;

        public IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            GetSiegeEquipmentByCharacterAndPool()
        {
            if (_onSessionLaunchedCachedObjectId is not null)
            {
                IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>? cachedNpcCharacters =
                    cacheProvider.GetCachedObject<IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>>(
                        _onSessionLaunchedCachedObjectId);
                if (cachedNpcCharacters is not null) return cachedNpcCharacters;

                _logger.Error("The cached equipment pools are null.");
                return new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>();
            }

            IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>> equipmentPools = equipmentRepositories
                .SelectMany(repo => repo.GetEquipmentPoolsById())
                .GroupBy(pools => pools.Key)
                .ToDictionary(
                    group => group.Key,
                    group =>
                    {
                        IList<Domain.EquipmentPool.Model.EquipmentPool> equipmentPools;
                        if (group.Count() > 1)
                        {
                            equipmentPools = group.First().Value;
                            _logger.Warn(
                                $"'{group.Key}' is defined in multiple xml files. Only the first equipment list will be used.");
                        }
                        else
                        {
                            equipmentPools = group.SelectMany(pool => pool.Value).ToList();
                        }

                        return new SiegeEquipmentSorter(equipmentPools).GetEquipmentPools();
                    });

            _onSessionLaunchedCachedObjectId = cacheProvider.CacheObject(equipmentPools);
            cacheProvider.InvalidateCache(_onSessionLaunchedCachedObjectId, CampaignEvent.OnAfterSessionLaunched);

            return equipmentPools;
        }
    }
}
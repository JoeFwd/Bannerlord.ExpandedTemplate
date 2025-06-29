using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Battle;

public class BattleEquipmentRosterProvider : IEquipmentRostersProvider
{
    private readonly ILogger _logger;
    private readonly ICacheProvider _cacheProvider;
    private readonly INpcCharacterWithResolvedEquipmentProvider _npcCharacterWithResolvedEquipmentProvider;
    private readonly IEquipmentRostersProvider _siegeEquipmentRosterProvider;
    private readonly IEquipmentRostersProvider _civilianEquipmentRosterProvider;

    private string? _onSessionLaunchedCachedObjectId;

    public BattleEquipmentRosterProvider(
        ILoggerFactory loggerFactory,
        ICacheProvider cacheProvider,
        IEquipmentRostersProvider siegeEquipmentRosterProvider,
        IEquipmentRostersProvider civilianEquipmentRosterProvider,
        INpcCharacterWithResolvedEquipmentProvider npcCharacterWithResolvedEquipmentProvider)
    {
        _logger = loggerFactory.CreateLogger<BattleEquipmentRosterProvider>();
        _cacheProvider = cacheProvider;
        _npcCharacterWithResolvedEquipmentProvider = npcCharacterWithResolvedEquipmentProvider;
        _siegeEquipmentRosterProvider = siegeEquipmentRosterProvider;
        _civilianEquipmentRosterProvider = civilianEquipmentRosterProvider;
    }

    public IDictionary<string, IList<EquipmentRoster>> GetEquipmentRostersByCharacter()
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

        IDictionary<string, IList<EquipmentRoster>> battleEquipmentRostersByCharacter =
            FilterOutNonBattleEquipmentRostersByCharacterId(_npcCharacterWithResolvedEquipmentProvider
                .GetNpcCharactersWithResolvedEquipmentRoster());

        _onSessionLaunchedCachedObjectId =
            _cacheProvider.CacheObject(battleEquipmentRostersByCharacter);
        _cacheProvider.InvalidateCache(_onSessionLaunchedCachedObjectId, CampaignEvent.OnAfterSessionLaunched);

        return battleEquipmentRostersByCharacter;
    }

    private IDictionary<string, IList<EquipmentRoster>> FilterOutNonBattleEquipmentRostersByCharacterId(
        IDictionary<string, IList<EquipmentRoster>> equipmentRostersByCharacterId)
    {
        IDictionary<string, IList<EquipmentRoster>> civilianEquipmentRostersByCharacter =
            _civilianEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        IDictionary<string, IList<EquipmentRoster>> siegeEquipmentRostersByCharacter =
            _siegeEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        return equipmentRostersByCharacterId
            .ToDictionary(character => character.Key, character => character.Value.Where(
                equipmentRoster =>
                {
                    if (bool.TryParse(equipmentRoster.IsBattle, out bool isBattle))
                        if (isBattle)
                            return true;

                    if (civilianEquipmentRostersByCharacter.TryGetValue(character.Key,
                            out IList<EquipmentRoster> civilianEquipmentRosters))
                        if (civilianEquipmentRosters.Contains(equipmentRoster))
                            return false;

                    if (siegeEquipmentRostersByCharacter.TryGetValue(character.Key,
                            out IList<EquipmentRoster> siegeEquipmentRosters))
                        if (siegeEquipmentRosters.Contains(equipmentRoster))
                            return false;

                    return true;
                }
            ).ToList() as IList<EquipmentRoster>);
    }
}
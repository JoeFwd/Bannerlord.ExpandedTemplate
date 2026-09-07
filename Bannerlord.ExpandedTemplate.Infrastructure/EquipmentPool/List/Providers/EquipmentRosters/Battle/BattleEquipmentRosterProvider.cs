using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Battle;

public class BattleEquipmentRosterProvider : IEquipmentRostersProvider
{
    private readonly BattleEquipmentRosterFilter _battleEquipmentRosterFilter;
    private readonly IEquipmentRostersProvider _civilianEquipmentRosterProvider;
    private readonly INpcCharacterWithResolvedEquipmentProvider _npcCharacterWithResolvedEquipmentProvider;
    private readonly IEquipmentRostersProvider _siegeEquipmentRosterProvider;
    private readonly IEquipmentRostersProvider _stealthEquipmentRosterProvider;

    public BattleEquipmentRosterProvider(
        IEquipmentRostersProvider siegeEquipmentRosterProvider,
        IEquipmentRostersProvider civilianEquipmentRosterProvider,
        IEquipmentRostersProvider stealthEquipmentRosterProvider,
        INpcCharacterWithResolvedEquipmentProvider npcCharacterWithResolvedEquipmentProvider,
        BattleEquipmentRosterFilter battleEquipmentRosterFilter)
    {
        _siegeEquipmentRosterProvider = siegeEquipmentRosterProvider;
        _civilianEquipmentRosterProvider = civilianEquipmentRosterProvider;
        _stealthEquipmentRosterProvider = stealthEquipmentRosterProvider;
        _npcCharacterWithResolvedEquipmentProvider = npcCharacterWithResolvedEquipmentProvider;
        _battleEquipmentRosterFilter = battleEquipmentRosterFilter;
    }

    public IDictionary<string, IList<EquipmentRoster>> GetEquipmentRostersByCharacter()
    {
        return _battleEquipmentRosterFilter.Filter(
            _npcCharacterWithResolvedEquipmentProvider.GetNpcCharactersWithResolvedEquipmentRoster(),
            _civilianEquipmentRosterProvider.GetEquipmentRostersByCharacter(),
            _siegeEquipmentRosterProvider.GetEquipmentRostersByCharacter(),
            _stealthEquipmentRosterProvider.GetEquipmentRostersByCharacter());
    }
}

using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Battle;

public class BattleEquipmentRosterProvider : IEquipmentRostersProvider
{
    private readonly INpcCharacterWithResolvedEquipmentProvider _npcCharacterWithResolvedEquipmentProvider;
    private readonly IEquipmentRostersProvider _siegeEquipmentRosterProvider;
    private readonly IEquipmentRostersProvider _civilianEquipmentRosterProvider;
    private readonly IEquipmentRostersProvider _stealthEquipmentRosterProvider;

    public BattleEquipmentRosterProvider(
        IEquipmentRostersProvider siegeEquipmentRosterProvider,
        IEquipmentRostersProvider civilianEquipmentRosterProvider,
        IEquipmentRostersProvider stealthEquipmentRosterProvider,
        INpcCharacterWithResolvedEquipmentProvider npcCharacterWithResolvedEquipmentProvider)
    {
        _npcCharacterWithResolvedEquipmentProvider = npcCharacterWithResolvedEquipmentProvider;
        _siegeEquipmentRosterProvider = siegeEquipmentRosterProvider;
        _civilianEquipmentRosterProvider = civilianEquipmentRosterProvider;
        _stealthEquipmentRosterProvider = stealthEquipmentRosterProvider;
    }

    public IDictionary<string, IList<EquipmentRoster>> GetEquipmentRostersByCharacter()
    {
        IDictionary<string, IList<EquipmentRoster>> battleEquipmentRostersByCharacter =
            FilterOutNonBattleEquipmentRostersByCharacterId(_npcCharacterWithResolvedEquipmentProvider
                .GetNpcCharactersWithResolvedEquipmentRoster());

        return battleEquipmentRostersByCharacter;
    }

    private IDictionary<string, IList<EquipmentRoster>> FilterOutNonBattleEquipmentRostersByCharacterId(
        IDictionary<string, IList<EquipmentRoster>> equipmentRostersByCharacterId)
    {
        IDictionary<string, IList<EquipmentRoster>> civilianEquipmentRostersByCharacter =
            _civilianEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        IDictionary<string, IList<EquipmentRoster>> siegeEquipmentRostersByCharacter =
            _siegeEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        IDictionary<string, IList<EquipmentRoster>> stealthEquipmentRostersByCharacter =
            _stealthEquipmentRosterProvider.GetEquipmentRostersByCharacter();

        return equipmentRostersByCharacterId
            .ToDictionary(character => character.Key, character => character.Value.Where(
                equipmentRoster =>
                {
                    if (bool.TryParse(equipmentRoster.ResolvedIsBattle, out bool isBattle))
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

                    if (stealthEquipmentRostersByCharacter.TryGetValue(character.Key,
                            out IList<EquipmentRoster> stealthEquipmentRosters))
                        if (stealthEquipmentRosters.Contains(equipmentRoster))
                            return false;

                    return true;
                }
            ).ToList() as IList<EquipmentRoster>);
    }
}

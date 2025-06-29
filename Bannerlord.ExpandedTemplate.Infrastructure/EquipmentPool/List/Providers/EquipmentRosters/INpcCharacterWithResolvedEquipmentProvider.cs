using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters;

public interface INpcCharacterWithResolvedEquipmentProvider
{
    IDictionary<string, IList<EquipmentRoster>> GetNpcCharactersWithResolvedEquipmentRoster();
}
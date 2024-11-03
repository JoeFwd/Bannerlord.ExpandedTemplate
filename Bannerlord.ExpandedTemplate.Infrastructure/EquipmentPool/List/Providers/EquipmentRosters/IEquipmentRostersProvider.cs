using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters;

public interface IEquipmentRostersProvider
{
    IDictionary<string, IList<EquipmentRoster>> GetEquipmentRostersByCharacter();
}
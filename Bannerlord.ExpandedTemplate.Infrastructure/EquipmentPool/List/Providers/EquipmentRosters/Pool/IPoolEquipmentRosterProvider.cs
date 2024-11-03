using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Pool;

public interface IPoolEquipmentRosterProvider
{
    IDictionary<string, IDictionary<string, IList<EquipmentRoster>>> GetEquipmentRostersByPoolAndCharacter();
}
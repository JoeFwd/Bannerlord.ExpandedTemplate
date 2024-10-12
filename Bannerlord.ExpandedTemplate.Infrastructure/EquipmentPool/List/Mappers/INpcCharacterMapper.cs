using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;

public interface INpcCharacterMapper
{
    IList<EquipmentRoster> MapToEquipmentRosters(NpcCharacter npcCharacter);
}
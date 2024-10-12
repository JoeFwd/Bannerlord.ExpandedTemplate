using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories;

public interface INpcCharacterRepository
{
    /// <summary>
    ///     Gets the NPC characters.
    /// </summary>
    /// <returns>Bannerlord's NPCCharacters</returns>
    /// <throws>TechnicalException when an errors occurs while getting the data</throws>
    NpcCharacters GetNpcCharacters();
}
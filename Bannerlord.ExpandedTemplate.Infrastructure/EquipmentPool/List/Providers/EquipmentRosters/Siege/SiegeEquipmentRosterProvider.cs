namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Siege;

public class SiegeEquipmentRosterProvider : FlaggedEquipmentRosterProvider
{
    public SiegeEquipmentRosterProvider(INpcCharacterWithResolvedEquipmentProvider npcCharacterWithResolvedEquipmentProvider)
        : base(npcCharacterWithResolvedEquipmentProvider, equipmentRoster => equipmentRoster.ResolvedIsSiege)
    {
    }
}

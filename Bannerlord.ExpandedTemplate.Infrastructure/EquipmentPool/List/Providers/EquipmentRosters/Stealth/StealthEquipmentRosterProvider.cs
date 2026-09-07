namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Stealth;

public class StealthEquipmentRosterProvider : FlaggedEquipmentRosterProvider
{
    public StealthEquipmentRosterProvider(INpcCharacterWithResolvedEquipmentProvider npcCharacterWithResolvedEquipmentProvider)
        : base(npcCharacterWithResolvedEquipmentProvider, equipmentRoster => equipmentRoster.ResolvedIsStealth)
    {
    }
}

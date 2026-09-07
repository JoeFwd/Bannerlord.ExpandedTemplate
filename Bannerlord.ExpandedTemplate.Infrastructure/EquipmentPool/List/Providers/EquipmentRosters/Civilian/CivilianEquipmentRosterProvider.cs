namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Civilian;

public class CivilianEquipmentRosterProvider : FlaggedEquipmentRosterProvider
{
    public CivilianEquipmentRosterProvider(INpcCharacterWithResolvedEquipmentProvider npcCharacterWithResolvedEquipmentProvider)
        : base(npcCharacterWithResolvedEquipmentProvider, equipmentRoster => equipmentRoster.ResolvedIsCivilian)
    {
    }
}

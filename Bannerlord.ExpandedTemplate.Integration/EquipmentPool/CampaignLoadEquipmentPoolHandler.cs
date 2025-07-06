using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;
using TaleWorlds.CampaignSystem;

namespace Bannerlord.ExpandedTemplate.Integration.EquipmentPool;

public class CampaignLoadEquipmentPoolHandler(
    ICacheInvalidator cacheInvalidator,
    IEquipmentPoolsProvider battleEquipmentPoolsProviders,
    IEquipmentPoolsProvider civilianEquipmentPoolsProviders,
    IEquipmentPoolsProvider siegeEquipmentPoolsProviders) : CampaignBehaviorBase
{
    public override void RegisterEvents()
    {
        CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, _ => OnSessionLaunched());
        CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, _ => OnAfterSessionLaunched());
    }

    public override void SyncData(IDataStore dataStore)
    {
    }

    private void OnSessionLaunched()
    {
        battleEquipmentPoolsProviders.GetEquipmentPoolsByCharacterId();
        civilianEquipmentPoolsProviders.GetEquipmentPoolsByCharacterId();
        siegeEquipmentPoolsProviders.GetEquipmentPoolsByCharacterId();
    }

    private void OnAfterSessionLaunched()
    {
        cacheInvalidator.InvalidateCache(CacheDataType.Xml);
    }
}
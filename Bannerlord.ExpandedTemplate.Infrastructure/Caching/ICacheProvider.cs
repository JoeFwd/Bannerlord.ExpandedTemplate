using System;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Caching
{
    public interface ICacheProvider
    {
        string CacheObject(object cacheObject);
        string CacheObject(Func<object> cacheRequest, CampaignEvent campaignEvent);
        T? GetCachedObject<T>(string id);
        void InvalidateCache();
        void InvalidateCache(string id);
        void InvalidateCache(string id, CampaignEvent campaignEvent);
    }
}

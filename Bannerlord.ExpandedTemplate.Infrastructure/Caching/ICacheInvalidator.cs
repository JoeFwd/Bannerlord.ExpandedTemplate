namespace Bannerlord.ExpandedTemplate.Infrastructure.Caching;

public interface ICacheInvalidator
{
    void InvalidateCache(CacheDataType cacheDataType);
}
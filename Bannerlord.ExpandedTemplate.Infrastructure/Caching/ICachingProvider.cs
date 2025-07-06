namespace Bannerlord.ExpandedTemplate.Infrastructure.Caching;

public interface ICachingProvider
{
    string CacheObject(object cacheObject, CacheDataType cacheDataType);

    T? GetObject<T>(string id);
}
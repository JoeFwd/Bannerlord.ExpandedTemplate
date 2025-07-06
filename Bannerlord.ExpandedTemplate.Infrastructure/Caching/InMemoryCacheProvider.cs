using System;
using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Caching;

public class InMemoryCacheProvider : ICachingProvider, ICacheInvalidator
{
    private readonly Dictionary<string, (CacheDataType, object)> _cache = new();

    public string CacheObject(object cacheObject, CacheDataType cacheDataType)
    {
        var id = GenerateCachedObjectId();
        _cache.Add(id, (cacheDataType, cacheObject));
        return id;
    }

    public T? GetObject<T>(string id)
    {
        if (_cache.TryGetValue(id, out var cacheEntry))
        {
            var (_, cacheObject) = cacheEntry;
            if (cacheObject is T typedObject) return typedObject;

            throw new InvalidCastException($"Cached object is not of type {typeof(T).Name}");
        }

        return default;
    }

    public void InvalidateCache(CacheDataType cacheDataType)
    {
        var keysToRemove = _cache
            .Where(entry => entry.Value.Item1 == cacheDataType)
            .Select(entry => entry.Key)
            .ToList();

        foreach (var key in keysToRemove) _cache.Remove(key);
    }

    private string GenerateCachedObjectId()
    {
        return Guid.NewGuid().ToString();
    }
}
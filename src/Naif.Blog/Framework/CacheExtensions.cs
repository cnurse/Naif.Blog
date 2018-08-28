using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Naif.Blog.Framework
{
    public static class CacheExtensions
    {
        public static T GetObject<T>(this IMemoryCache memoryCache, string cacheKey, ILogger logger, MemoryCacheEntryOptions cacheOptions, Func<T> refreshCache)
        {
            if (!memoryCache.TryGetValue(cacheKey, out T cachedObject))
            {
                // fetch the value from the source
                cachedObject = refreshCache();

                // store in the cache
                memoryCache.Set(cacheKey, cachedObject, cacheOptions);
                
                logger.LogInformation($"{cacheKey} updated from source.");
            }
            else
            {
                logger.LogInformation($"{cacheKey} retrieved from cache.");
            }

            return cachedObject;
        }
    }
}
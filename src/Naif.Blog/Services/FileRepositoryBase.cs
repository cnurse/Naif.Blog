using System;
using System.Collections.Generic;
using Naif.Blog.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Naif.Blog.Services
{
    public abstract class FileRepositoryBase
    {

        protected FileRepositoryBase(IHostingEnvironment env, IMemoryCache memoryCache)
        {
            MemoryCache = memoryCache;
            FilesFolder = "/posts/{0}/files/";
            RootFolder = env.WebRootPath;
        }

        protected abstract string FileExtension { get; }

        protected string FilesFolder { get; }

        protected ILogger Logger {get; set;}

        protected IMemoryCache MemoryCache { get; }

        protected string RootFolder { get; }

        protected void DeleteObject<T>(T obj, string id, string cacheKeyTemplate, string folder) where T : PostBase
        {
            var cacheKey = GetCacheKey(cacheKeyTemplate, obj.BlogId);
            var objFolder = GetFolder(folder, obj.BlogId);
            string file = Path.Combine(objFolder, id + "." + FileExtension);

            File.Delete(file);

            MemoryCache.Remove(cacheKey);
            Logger.LogInformation($"{cacheKey} cleared.");
        }

        private string GetCacheKey(string cachekey, string blogId)
        {
            return string.Format(cachekey, blogId);
        }

        private string GetFolder(string folderKey, string blogId)
        {
            return string.Format(folderKey, RootFolder, blogId);
        }
        
        protected IList<T> GetObjects<T>(string cacheKeyTemplate, string folderKey, string blogId, Func<string, string, T> func) where T : PostBase
        {
            var cacheKey = GetCacheKey(cacheKeyTemplate, blogId);
            IList<T> list;
            
            if (!MemoryCache.TryGetValue(cacheKey, out list))
            {
                // fetch the value from the source
                list = GetObjects(folderKey, blogId, (f, id) => func(f, id)).ToList();

                // store in the cache
                MemoryCache.Set(cacheKey,
                    list,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(2)));
                Logger.LogInformation($"{cacheKey} updated from source.");
            }
            else
            {
                Logger.LogInformation($"{cacheKey} retrieved from cache.");
            }

            return list;
        }

        private IList<T> GetObjects<T>(string folder, string blogId, Func<string, string, T> func) where T : PostBase 
        {
            var objFolder = GetFolder(folder, blogId);

            if (!Directory.Exists(objFolder))
            {
                Directory.CreateDirectory(objFolder);
            }

            List<T> list = new List<T>();

            // Can this be done in parallel to speed it up?
            foreach (string file in Directory.EnumerateFiles(objFolder, "*." + FileExtension, SearchOption.TopDirectoryOnly))
            {
                list.Add(func(file, blogId));
            }

            if (list.Count > 0)
            {
                list = list.OrderByDescending(p => p.PubDate).ToList();
            }

            return list;
        }

        protected void SaveObject<T>(T obj, string id, string cacheKeyTemplate, string folder, Action<T, string> action) where T : PostBase 
        {
            var cacheKey = GetCacheKey(cacheKeyTemplate, obj.BlogId);
            var objFolder = GetFolder(folder, obj.BlogId);

            string file = Path.Combine(objFolder, id + "." + FileExtension);
            obj.LastModified = DateTime.UtcNow;

            action(obj, file);

            MemoryCache.Remove(cacheKey);

            Logger.LogInformation(!File.Exists(file)
                ? $"New Page - {id} created."
                : $"Page - {id} updated.");

            Logger.LogInformation($"{cacheKey} cleared.");
        }

    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Naif.Blog.Framework
{
    /// <summary>
    /// The ApplicationContextMiddleware component processes the request to built the ApplicationContext object used
    /// for the duration of the request.  
    /// The IApplicationContext object is passed in by Dependency Injection with a "scoped" lifetime, ensuring that a new object
    /// is created for each request.
    /// </summary>
    public class ApplicationContextMiddleware
    {
        RequestDelegate _next;
        private ILogger _logger;
        private IMemoryCache _memoryCache;
        private string _blogsCacheKey = "blogs";
        private string _blogsFile;

        public ApplicationContextMiddleware(RequestDelegate next, 
                                    IHostingEnvironment env, 
                                    IMemoryCache memoryCache,
                                    ILoggerFactory loggerFactory)
        {
            _memoryCache = memoryCache;
            _logger = loggerFactory.CreateLogger<ApplicationContextMiddleware>();
            _next = next;
            _blogsFile = env.WebRootPath + "/blogs.json";
        }

        public async Task Invoke(HttpContext context, IApplicationContext appContext)
        {
            appContext.Blogs = GetBlogs();
            appContext.CurrentBlog = appContext.Blogs.SingleOrDefault(b => b.Url == context.Request.Host.Value);
            await _next.Invoke(context);
        }

        private IEnumerable<Models.Blog> GetBlogs()
        {
            IEnumerable<Models.Blog> blogs;

            if (!_memoryCache.TryGetValue(_blogsCacheKey, out blogs))
            {
                // fetch the value from the source
                using (StreamReader reader = File.OpenText(_blogsFile))
                {
                    var json = reader.ReadToEnd();
                    blogs = JsonConvert.DeserializeObject<IEnumerable<Models.Blog>>(json);
                }

                // store in the cache
                _memoryCache.Set(_blogsCacheKey,
                    blogs,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(2)));
                _logger.LogInformation($"{_blogsCacheKey} updated from source.");
            }
            else
            {
                _logger.LogInformation($"{_blogsCacheKey} retrieved from cache.");
            }

            return blogs;
        }
    }
}


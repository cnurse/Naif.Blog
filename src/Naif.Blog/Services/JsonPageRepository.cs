using Naif.Blog.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Newtonsoft.Json;

namespace Naif.Blog.Services
{
    public class JsonPageRepository : FilePageRepository
    {
        public JsonPageRepository(IWebHostEnvironment env, IMemoryCache memoryCache, ILoggerFactory loggerFactory) : base(env, memoryCache)
        {
            Logger = loggerFactory.CreateLogger<JsonPostRepository>();
        }

        protected override string FileExtension => "json";

        protected override Page GetPage(string file, string blogId)
        {
            Page page;
            using (StreamReader r = File.OpenText(file))
            {
                string json = r.ReadToEnd();
                page =  JsonConvert.DeserializeObject<Page>(json);
                page.BlogId = blogId;
            }
            return page;
        }

        protected override void SavePage(Page page, string file)
        {
            using (StreamWriter w = File.CreateText(file))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(w, page);
            }
        }
    }
}

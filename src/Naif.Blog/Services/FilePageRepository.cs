using System.Collections.Generic;
using Naif.Blog.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using System.Linq;
// ReSharper disable ConvertClosureToMethodGroup

namespace Naif.Blog.Services
{
    public abstract class FilePageRepository : FileRepositoryBase, IPageRepository
    {
        private readonly string _pagesCacheKey;
        private readonly string _pagesFolder;

        protected FilePageRepository(IWebHostEnvironment env, IMemoryCache memoryCache) : base(env, memoryCache)
        {
            _pagesCacheKey = "{0}_pages";
            _pagesFolder = Path.Combine("{0}", "pages", "{1}");
        }

        public void DeletePage(Page page)
        {
            DeleteObject(page, page.PageId, _pagesCacheKey, _pagesFolder);
        }

        public IEnumerable<Page> GetAllPages(string blogId)
        {
            return GetObjects(_pagesCacheKey, _pagesFolder, blogId, (f, id) => GetPage(f, id)).ToList();
        }

        protected abstract Page GetPage(string file, string blogId);

        public void SavePage(Page page)
        {
            SaveObject(page, page.PageId, _pagesCacheKey, _pagesFolder,  (p, f) => SavePage(p, f));
        }
        
        protected abstract void SavePage(Page page, string file);
    }
}

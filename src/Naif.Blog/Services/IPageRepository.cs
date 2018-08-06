using Naif.Blog.Models;
using System.Collections.Generic;

namespace Naif.Blog.Services
{
    public interface IPageRepository
    {
        void DeletePage(Page page);

        IEnumerable<Page> GetAllPages(string blogId);

        void SavePage(Page page);
    }
}

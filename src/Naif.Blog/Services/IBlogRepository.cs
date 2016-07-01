using Naif.Blog.Models;
using System.Collections.Generic;

namespace Naif.Blog.Services
{
    public interface IBlogRepository
    {
        void Delete(Post post);

        IEnumerable<Post> GetAll();

        Dictionary<string, int> GetCategories();

        Dictionary<string, int> GetTags();

        void Save(Post post);
    }
}

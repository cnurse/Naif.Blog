using Naif.Blog.Models;
using System.Collections.Generic;

namespace Naif.Blog.Services
{
    public interface IBlogRepository
    {
        IEnumerable<Post> GetAll();
    }
}

using System.Collections.Generic;
using Naif.Blog.Models;

namespace Naif.Blog.ViewModels
{
    public class BlogViewModel
    {
        public Naif.Blog.Models.Blog Blog { get; set; }
        
        public int Page { get; set; }
        
        public Post Post { get; set; }

        public IEnumerable<Post> Posts { get; set; }
    }
}
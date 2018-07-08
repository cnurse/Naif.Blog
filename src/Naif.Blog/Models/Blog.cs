using System.Collections.Generic;

namespace Naif.Blog.Models
{
    public class Blog
    {
        public string ByLine { get; set; }

        public string Disclaimer { get; set; }

        public string Id { get; set; }

        public string LocalUrl { get; set; }

        public Post Post { get; set; }

        public IEnumerable<Post> Posts { get; set; }
        
        public string Theme { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }
    }
}



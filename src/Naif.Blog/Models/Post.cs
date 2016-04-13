using System;

namespace Naif.Blog.Models
{
    public class Post
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Slug { get; set; }

        public string Excerpt { get; set; }

        public string Content { get; set; }

        public DateTime PubDate { get; set; }

        public DateTime LastModified { get; set; }

        public bool IsPublished { get; set; }
    }
}

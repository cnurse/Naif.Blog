using System;
using System.Collections.Generic;

namespace Naif.Blog.Framework
{
    public class ApplicationContext : IApplicationContext
    {
        public ApplicationContext()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public IEnumerable<Models.Blog> Blogs { get; set; }

        public Models.Blog CurrentBlog { get; set; }
    }
}

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Naif.Blog.Models;

namespace Naif.Blog.ViewModels
{
    public class BlogViewModel
    {
        public Naif.Blog.Models.Blog Blog { get; set; }
        
        public List<SelectListItem> Categories { get; set; }
        
        public string Filter { get; set; }
        
        public int PageIndex { get; set; }
        
        public Page Page { get; set; }

        public IEnumerable<Page> Pages { get; set; }

        public Post Post { get; set; }

        public IEnumerable<Post> Posts { get; set; }
        
        public List<SelectListItem> Themes { get; set; }
    }
}
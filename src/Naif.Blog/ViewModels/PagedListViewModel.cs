using System.Collections.Generic;
using Naif.Blog.Models;

namespace Naif.Blog.ViewModels
{
    public class PagedListViewModel
    {
        public bool IsPage { get; set; }
        public IEnumerable<PostBase> Posts { get; set; }
        public Pager Pager { get; set; }
    }
}
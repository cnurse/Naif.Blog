using System.Collections.Generic;

namespace Naif.Blog.Models
{
    public class Pager
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public string CssClass { get; set; }
        public string Filter { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public string NextCssClass { get; set; }
        public string NextText { get; set; }
        public int PageCount { get; set; }
        public int PageIndex { get; set; }
        public string PreviousCssClass { get; set; }
        public string PreviousText { get; set; }
        public Dictionary<string, object> RouteValues { get; set; }
    }
}
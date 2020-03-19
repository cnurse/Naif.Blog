using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Naif.Blog.Framework;
using Naif.Blog.Models;
using Naif.Blog.ViewModels;

namespace Naif.Blog.ViewComponents
{
    public class PagedListViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<PostBase> list, int pageCount, int pageIndex, string filter, bool isPage, bool? isTable)
        {
            var posts = list.InPagesOf(pageCount).GetPage(pageIndex);
            
            string actionName = ViewContext.ActionDescriptor.RouteValues["action"];
            string controller = ViewContext.ActionDescriptor.RouteValues["controller"];
            string actionParameter = actionName == "Index" 
                ? String.Empty
                : actionName == "ViewCategory" 
                    ? "category"
                    : "tag";
            string actionValue = actionName == "Index"
                ? String.Empty
                : ViewContext.RouteData.Values[actionParameter] as string;

            PagedListViewModel viewModel = null;

            await Task.Run(() =>
            {
                viewModel = new PagedListViewModel
                {
                    IsPage = isPage,
                    Pager = new Pager
                    {
                        Action = actionName,
                        Controller = controller,
                        CssClass = "pagination",
                        Filter = filter,
                        HasNextPage = posts.HasNextPage,
                        HasPreviousPage = posts.HasPreviousPage,
                        NextCssClass = "right",
                        NextText = "Next",
                        PageCount = posts.PageCount,
                        PageIndex = posts.PageIndex,
                        PreviousCssClass = "left",
                        PreviousText = "Previous",
                        RouteValues = new Dictionary<string, object> {[actionParameter] = actionValue}
                    },
                    Posts = posts
                };
            });

            // ReSharper disable once Mvc.ViewComponentViewNotResolved
            return isTable.HasValue && isTable.Value ? View("Table", viewModel) : View(viewModel);
        }
    }
}

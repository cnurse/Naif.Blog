//Inspired by the work of Michael McKenna
//https://michael-mckenna.com/implementing-xml-rpc-services-in-asp-net-mvc/

using Microsoft.AspNet.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Naif.Blog.Routing
{
    public static class RouteBuilderExtensions
    {
        public static IRouteBuilder MapMetaWeblogRoute(this IRouteBuilder routeCollectionBuilder)
        {
            var inlineConstraintResolver = routeCollectionBuilder
                                                       .ServiceProvider
                                                       .GetService<IInlineConstraintResolver>();

            routeCollectionBuilder.Routes.Add(new MetaWeblogRoute(routeCollectionBuilder.DefaultHandler,
                                                                "metaweblog",
                                                                "api/metaweblog",
                                                                new RouteValueDictionary(),
                                                                new RouteValueDictionary(),
                                                                new RouteValueDictionary(),
                                                                inlineConstraintResolver));

            return routeCollectionBuilder;
        }
    }
}

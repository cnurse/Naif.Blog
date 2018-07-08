//Inspired by the work of Michael McKenna
//https://michael-mckenna.com/implementing-xml-rpc-services-in-asp-net-mvc/
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Naif.Blog.Routing
{
    public class MetaWeblogRoute : Route
    {
        public MetaWeblogRoute(IRouter target,
                            string routeName,
                            string routeTemplate,
                            RouteValueDictionary defaults,
                            IDictionary<string, object> constraints,
                            RouteValueDictionary dataTokens,
                            IInlineConstraintResolver inlineConstraintResolver)
            : base(target, routeName, routeTemplate, defaults, 
                  constraints, dataTokens, inlineConstraintResolver)
        { }

        public override async Task RouteAsync(RouteContext context)
        {
            if (context.HttpContext.Request.Body != null 
                    && context.HttpContext.Request.ContentLength != null 
                    && context.HttpContext.Request.ContentLength > 0)
            {
                XDocument xDoc = XDocument.Load(context.HttpContext.Request.Body);
                string methodName = xDoc.Document
                                        .Element("methodCall")
                                        .Element("methodName")
                                        .Value;
                var methodNameParts = methodName.Split('.');

                context.RouteData.Values["controller"] = "MetaWeblog";
                context.RouteData.Values["action"] = methodNameParts[1];
                context.HttpContext.Items["Xml-Rpc-Document"] = xDoc;
            }

            await base.RouteAsync(context);
        }
    }
}


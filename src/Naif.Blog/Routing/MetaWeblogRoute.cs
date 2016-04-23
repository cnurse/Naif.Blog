//Inspired by the work of Michael McKenna
//https://michael-mckenna.com/implementing-xml-rpc-services-in-asp-net-mvc/
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Naif.Blog.Routing
{
    public class MetaWeblogRoute : TemplateRoute
    {
        public MetaWeblogRoute(IRouter target,
                             string routeName,
                             string routeTemplate,
                             IDictionary<string, object> defaults,
                             IDictionary<string, object> constraints,
                             IDictionary<string, object> dataTokens,
                             IInlineConstraintResolver inlineConstraintResolver)
            : base(target, routeName, routeTemplate, defaults, 
                  constraints, dataTokens, inlineConstraintResolver)
        { }

        public async override Task RouteAsync(RouteContext context)
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


//Inspired by the work of Michael McKenna
//https://michael-mckenna.com/implementing-xml-rpc-services-in-asp-net-mvc/
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Xml.Linq;

namespace Naif.Blog.XmlRpc
{
    public class XmlRpcServiceAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            XDocument xDoc = filterContext.HttpContext.Items["Xml-Rpc-Document"] as XDocument;

            //Set the parameter values from the XML-RPC request XML
            if (xDoc != null)
            {
                var xmlParams = xDoc.Document.Element("methodCall")
                                                .Element("params")
                                                .Elements("param")
                                                .ToArray();
                int index = 0;
                foreach (var paramDescriptor in filterContext.ActionDescriptor.Parameters)
                {
                    var node = xmlParams[index].Element("value");
                    filterContext.ActionArguments[paramDescriptor.Name] 
                                = XmlRpcData.DeserialiseValue(node, paramDescriptor.ParameterType);
                    index++;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}


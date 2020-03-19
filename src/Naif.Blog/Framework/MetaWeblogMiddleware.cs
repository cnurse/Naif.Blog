using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Naif.Blog.XmlRpc;

namespace Naif.Blog.Framework
{
    /// <summary>
    /// The MetaWeblogMiddleware component processes the request to support MetaWeblog API support
    /// </summary>
    public class MetaWeblogMiddleware
    {
        private readonly RequestDelegate _next;

        public MetaWeblogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;

            if (request.Path == "/MetaWeblog")
            {
                if (request.Body != null 
                    && request.ContentLength != null 
                    && request.ContentLength > 0)
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    CancellationToken token = source.Token;
                    XDocument xDoc = await XDocument.LoadAsync(context.Request.Body, new LoadOptions(), token);
                    string methodName = xDoc.Document
                        .Element("methodCall")
                        .Element("methodName")
                        .Value;
                    var method = methodName.Split('.')[1];
                    
                    var xmlParams = xDoc.Document.Element("methodCall")
                        .Element("params")
                        .Elements("param")
                        .ToArray();
                    
                    var methodParams = new List<object>();
                    
                    foreach (var xmlParam in xmlParams)
                    {
                        Type type = (method == "editPost" || method == "newPost") ? Type.GetType("Naif.Blog.Models.Post") :
                            (method == "editPage" || method == "newPage") ? Type.GetType("Naif.Blog.Models.Page") :
                            (method == "newMediaObject") ? Type.GetType("Naif.Blog.Models.MediaObject") : null;
                        
                        methodParams.Add(XmlRpcData.DeserialiseValue(xmlParam.Element("value"), type));
                    }

                    context.Items["Xml-Rpc-Document"] = xDoc;
                    context.Items["Xml-Rpc-MethodName"] = method;
                    context.Items["Xml-Rpc-Parameters"] = methodParams;
                }
            }
            await _next.Invoke(context);
        }
    }
}
//Inspired by the work of Michael McKenna
//https://michael-mckenna.com/implementing-xml-rpc-services-in-asp-net-mvc/

using Microsoft.AspNetCore.Mvc;
using System;
using System.Xml.Linq;

namespace Naif.Blog.XmlRpc
{
    public class XmlRpcResult : ContentResult
    {
        public XmlRpcResult(object data)
        {
            //Set content type to xml
            ContentType = "text/xml";

            //Serialise data into base.Content
            Content = SerialiseXmlRpcResponse(data).ToString();
        }

        private XDocument SerialiseXmlRpcResponse(object data)
        {
            var exception = data as Exception;

            if (exception != null)
            {
                return new XDocument(
                        new XElement("methodResponse",
                            new XElement("fault",
                                new XElement("value",
                                    new XElement("string", exception.Message)
                                )
                            )
                        )
                    );
            }
            else
            {
                return new XDocument(
                        new XElement("methodResponse",
                            new XElement("params",
                                new XElement("param", XmlRpcData.SerialiseValue(data))
                            )
                        )
                    );
            }
        }
    }
}



using System;

namespace Naif.Blog.XmlRpc
{
    public class XmlRpcPropertyAttribute : Attribute
    {
        public XmlRpcPropertyAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}

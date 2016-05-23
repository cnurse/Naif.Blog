using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

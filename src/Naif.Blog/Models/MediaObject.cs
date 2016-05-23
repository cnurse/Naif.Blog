using Naif.Blog.XmlRpc;

namespace Naif.Blog.Models
{
    public class MediaObject
    {
        [XmlRpcProperty("name")]
        public string Name { get; set; }

        [XmlRpcProperty("type")]
        public string Type { get; set; }

        [XmlRpcProperty("bits")]
        public byte[] Bits { get; set; }
    }
}

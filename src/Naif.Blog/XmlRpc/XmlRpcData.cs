//Inspired by the work of Michael McKenna
//https://michael-mckenna.com/implementing-xml-rpc-services-in-asp-net-mvc/
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Naif.Blog.XmlRpc
{
    public static class XmlRpcData
    {
        public static object DeserialiseValue(XElement value, Type targetType)
        {
            //First check that we have a 'value' node that's been passed in
            if (!String.Equals(value.Name.LocalName, "value", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The supplied node is not a 'value' node.");

            var dataTypeNode = value.Elements().Single();
            var dataType = dataTypeNode.Name.LocalName;

            if (dataType == "string")
            {
                return value.Value;
            }
            else if (dataType == "int" || dataType == "i4")
            {
                return int.Parse(value.Value);
            }
            else if (dataType == "i8")
            {
                return Int64.Parse(value.Value);
            }
            else if (dataType == "double")
            {
                return double.Parse(value.Value);
            }
            else if (dataType == "boolean")
            {
                return value.Value == "1";
            }
            else if (dataType == "dateTime.iso8601")
            {
                return DateTime.ParseExact(value.Value, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            else if (dataType== "base64")
            {
                return Convert.FromBase64String(value.Value);
            }
            else if (dataType == "array")
            {
                var entries = value.Element("array").Element("data").Elements("value");
                var xElements = entries as XElement[] ?? entries.ToArray();
                var targetArray = Array.CreateInstance(targetType.GetElementType(), xElements.Length);
                int index = 0;
                foreach (var entry in xElements)
                {
                    var propertyValue = DeserialiseValue(entry, targetType.GetElementType());
                    targetArray.SetValue(propertyValue, index);
                    index++;
                }
                return targetArray;
            }
            else if (dataType == "struct")
            {
                var members = value.Element("struct").Elements("member");
                object targetObject = Activator.CreateInstance(targetType);
                var propertyInfos = targetType.GetProperties();
                foreach (var member in members)
                {
                    foreach(var propertyInfo in propertyInfos)
                    {
                        if(propertyInfo.Name == member.Element("name").Value)
                        {
                            SetPropertyValue(propertyInfo, targetObject, member);
                        }
                        else
                        {
                            var attribute = propertyInfo.GetCustomAttribute(typeof(XmlRpcPropertyAttribute));
                            var xmlRpcAttribute = attribute as XmlRpcPropertyAttribute;
                            if (xmlRpcAttribute != null && xmlRpcAttribute.Name == member.Element("name").Value)
                            {
                                SetPropertyValue(propertyInfo, targetObject, member);
                            }
                        }
                    }
                }
                return targetObject;
            }
            else
            {
                throw new ArgumentException(String.Format("The supplied XML-RPC value '{0}' is not recognised", dataType));
            }
        }

        private static void SetPropertyValue(PropertyInfo propertyInfo, object targetObject, XElement member)
        {
            var propertyValue = DeserialiseValue(member.Element("value"), propertyInfo.PropertyType);
            propertyInfo.SetValue(targetObject, propertyValue, null);
        }

        public static XElement SerialiseValue(object value)
        {
            var root = new XElement("value");

            if (value == null)
            {
                root.Add(new XElement("nil"));
            }
            else if (IsPrimitiveXmlRpcType(value.GetType()))
            {
                root.Add(SerialisePrimitive(value));
            }
            else if (value.GetType().IsArray)
            {
                root.Add(SerialiseEnumerable(value as IEnumerable));
            }
            else
            {
                root.Add(SerialiseStrut(value));
            }

            return root;
        }

        private static bool IsPrimitiveXmlRpcType(Type type)
        {
            return type.GetTypeInfo().IsPrimitive || type == typeof(String) || type == typeof(DateTime) || type == typeof(Int64);
        }

        private static XElement SerialiseStrut(object value)
        {
            XElement root = new XElement("struct");

            PropertyInfo[] propInfos = value.GetType().GetProperties();
            foreach (PropertyInfo propInfo in propInfos)
            {
                XElement member = new XElement("member");

                member.Add(
                    new XElement("name", propInfo.Name), SerialiseValue(propInfo.GetValue(value, null)));

                root.Add(member);
            }

            return root;
        }

        private static XElement SerialiseEnumerable(IEnumerable values)
        {
            XElement enumerableElement = new XElement("array");
            XElement dataElement = new XElement("data");

            foreach (var value in values)
            {
                dataElement.Add(SerialiseValue(value));
            }

            enumerableElement.Add(dataElement);

            return enumerableElement;
        }

        private static XElement SerialisePrimitive(object value)
        {
            if (value is string)
            {
                return new XElement("string", value);
            }
            else if (value is int)
            {
                return new XElement("int", value.ToString());
            }
            else if (value is Int64)
            {
                return new XElement("i8", value.ToString());
            }
            else if (value is double)
            {
                return new XElement("double", value.ToString());
            }
            else if (value is bool)
            {
                return new XElement("boolean", (bool)value ? "1" : "0");
            }
            else if (value is DateTime)
            {
                return new XElement("dateTime.iso8601", ((DateTime)value).ToString("s"));
            }

            throw new ArgumentException(String.Format("We cannot encode the primitive '{0}'", value.GetType()));
        }

        //iso8601 often come in slightly different flavours rather than the standard "s" that string.format supports.
        //http://stackoverflow.com/a/17752389
        static readonly string[] Formats = {
            // Basic formats
            "yyyyMMddTHHmmsszzz",
            "yyyyMMddTHHmmsszz",
            "yyyyMMddTHHmmssZ",
            // Extended formats
            "yyyy-MM-ddTHH:mm:sszzz",
            "yyyy-MM-ddTHH:mm:sszz",
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyyMMddTHH:mm:ss:zzz",
            "yyyyMMddTHH:mm:ss:zz",
            "yyyyMMddTHH:mm:ss:Z",
            "yyyyMMddTHH:mm:ss",
            // All of the above with reduced accuracy
            "yyyyMMddTHHmmzzz",
            "yyyyMMddTHHmmzz",
            "yyyyMMddTHHmmZ",
            "yyyy-MM-ddTHH:mmzzz",
            "yyyy-MM-ddTHH:mmzz",
            "yyyy-MM-ddTHH:mmZ",
            // Accuracy reduced to hours
            "yyyyMMddTHHzzz",
            "yyyyMMddTHHzz",
            "yyyyMMddTHHZ",
            "yyyy-MM-ddTHHzzz",
            "yyyy-MM-ddTHHzz",
            "yyyy-MM-ddTHHZ"
        };
    }
}

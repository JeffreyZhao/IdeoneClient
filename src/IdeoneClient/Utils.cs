using System;
using System.Collections;
using System.Xml;

namespace IdeoneClient
{
    internal class Utils
    {
        public static Hashtable ParseXmlNodes(XmlNode[] nodes)
        {
            var hashtable = new Hashtable();

            for (var i = 1; i < nodes.Length; i++)
            {
                ParseKeyValue(hashtable, nodes[i]);
            }

            return hashtable;
        }

        private static void ParseKeyValue(Hashtable hashtable, XmlNode node)
        {
            var key = ParseValue(node["key"]);
            var value = ParseValue(node["value"]);
            hashtable.Add(key, value);
        }

        private static object ParseValue(XmlNode node)
        {
            var type = node.Attributes["xsi:type"].Value;
            switch (type)
            {
                case "xsd:string":
                    return node.InnerText;
                case "xsd:int":
                    return Int32.Parse(node.InnerText);
                case "xsd:boolean":
                    return Boolean.Parse(node.InnerText);
                case "xsd:float":
                    return Single.Parse(node.InnerText);
                case "ns2:Map":
                    return ParseMap(node);
                default:
                    throw new ArgumentException("Unexpected type: " + type);
            }
        }

        private static Hashtable ParseMap(XmlNode node)
        {
            var hashtable = new Hashtable();

            foreach (XmlNode child in node.ChildNodes)
            {
                ParseKeyValue(hashtable, child);
            }

            return hashtable;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using Xunit;
using System.Collections;

namespace IdeoneClient.Tests
{
    internal class UtilsTest
    {
        public class ParseXmlNode : UtilsTest
        {
            private XElement XElement(string name, string type, string value)
            {
                XNamespace xsi = "xsi";
                return
                    new XElement(name,
                        new XAttribute(xsi + "type", type),
                        new XText(value));
            }

            private XElement KeyValue(string key, string type, string value)
            {
                return
                    new XElement("item",
                        XElement("key", "xsd:string", key),
                        XElement("value", type, value));
            }

            [Fact]
            public void PrimitiveTypes()
            {
                var xml =
@"<root xmlns:xsi=""http://example.com/"" xmlns:xsd=""http://example.com/"">
    <ignored />
    <item>
        <key xsi:type=""xsd:string"">string_key</key>
        <value xsi:type=""xsd:string"">string_value</value>
    </item>
    <item>
        <key xsi:type=""xsd:int"">5</key>
        <value xsi:type=""xsd:int"">10</value>
    </item>
    <item>
        <key xsi:type=""xsd:string"">boolean_key</key>
        <value xsi:type=""xsd:boolean"">true</value>
    </item>
    <item>
        <key xsi:type=""xsd:string"">float_key</key>
        <value xsi:type=""xsd:float"">1.2</value>
    </item>
</root>";

                var doc = new XmlDocument();
                doc.LoadXml(xml);

                var hashtable = Utils.ParseXmlNodes(doc.DocumentElement.ChildNodes.Cast<XmlNode>().ToArray());

                Assert.Equal("string_value", hashtable["string_key"]);
                Assert.Equal(true, hashtable["boolean_key"]);
                Assert.Equal(10, hashtable[5]);
                Assert.Equal(1.2F, hashtable["float_key"]);
            }

            [Fact]
            public void Map()
            {
                var xml =
@"<root xmlns:xsi=""http://example.com/"" xmlns:xsd=""http://example.com/"">
    <ignored />
    <item>
        <key xsi:type=""xsd:string"">map</key>
        <value xsi:type=""ns2:Map"">
            <item>
                <key xsi:type=""xsd:string"">string_key</key>
                <value xsi:type=""xsd:string"">string_value</value>
            </item>
            <item>
                <key xsi:type=""xsd:string"">int_key</key>
                <value xsi:type=""xsd:int"">10</value>
            </item>
            <item>
                <key xsi:type=""xsd:string"">boolean_key</key>
                <value xsi:type=""xsd:boolean"">true</value>
            </item>
            <item>
                <key xsi:type=""xsd:string"">float_key</key>
                <value xsi:type=""xsd:float"">1.2</value>
            </item>
        </value>
    </item>
</root>";

                var doc = new XmlDocument();
                doc.LoadXml(xml);

                var hashtable = Utils.ParseXmlNodes(doc.DocumentElement.ChildNodes.Cast<XmlNode>().ToArray());
                hashtable = (Hashtable)hashtable["map"];

                Assert.Equal("string_value", hashtable["string_key"]);
                Assert.Equal(true, hashtable["boolean_key"]);
                Assert.Equal(10, hashtable["int_key"]);
                Assert.Equal(1.2F, hashtable["float_key"]);
            }
        }
    }
}

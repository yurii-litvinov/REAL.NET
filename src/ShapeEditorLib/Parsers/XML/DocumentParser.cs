using System;
using System.Collections.Generic;
using System.Xml;

namespace ShapeEditorLib.Parsers.XML
{
    public class XMLDocumentParser
    {
        private XmlDocument document;

        public XMLDocumentParser(XmlDocument document)
        {
            this.document = document;
        }

        public string GetName()
        {
            var list = document.SelectNodes("/Element");
            if (list.Count != 1)
            {
                throw new InvalidOperationException("Incorrect format: can't find tag Element or it's more tha one");
            }
            XmlNode element = list[0];
            string name = element.Attributes["Name"].Value;
            return name;
        }

        public IDictionary<string, string> GetProperties()
        {
            var properties = new Dictionary<string, string>();
            foreach (XmlNode propertyNode in document.SelectNodes("/Element/Properties/Property"))
            {
                string name = propertyNode.Attributes["Name"].Value;
                string value = propertyNode.Attributes["Value"].Value;
                properties.Add(name, value);
            }
            return properties;
        }

        public IEnumerable<string> GetAttributesNames()
        {
            var names = new List<string>();
            foreach (XmlNode attributeNode in document.SelectNodes("/Element/Attributes/Attribute"))
            {
                string name = attributeNode.Attributes["Name"].Value;
                names.Add(name);
            }
            return names;
        }

        public IDictionary<string, string> GetAttributeProperties(string attributeName)
        {
            var properties = new Dictionary<string, string>();
            XmlNode found = null;
            int count = 0;
            foreach (XmlNode attributeNode in document.SelectNodes("/Element/Attributes/Attribute"))
            {
                string name = attributeNode.Attributes["Name"].Value;
                if (name == attributeName)
                {
                    found = attributeNode;
                    ++count;
                }
            }
            if (count > 1)
            {
                throw new InvalidOperationException("Ambiguous attribute: more than one definition");
            }
            foreach (XmlNode property in found.ChildNodes)
            {
                string name = property.Name;
                string value = property.InnerText;
                properties.Add(name, value);
            }
            return properties;
        }
    }
}

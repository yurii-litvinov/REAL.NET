using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ShapeEditorLib.Parsers
{
    /// <summary>
    /// This class parses XML documents and extract info form them.
    /// </summary>
    public class XMLParser : IViewFileParser
    {
        private DocumentParser documentParser;

        /// <summary>
        /// Loads file.
        /// </summary>
        /// <param name="link">Link of file to load.</param>
        /// <exception cref="System.Xml.XmlException">There is a load or parse error in the XML.</exception>
        /// <exception cref="ArgumentException">There is incorrect path or permission</exception>
        public void LoadFile(string link)
        {
            var document = new XmlDocument();
            document.Load(link);
            documentParser = new DocumentParser(document);
        }

        /// <summary>
        /// Gets info about attribute.
        /// </summary>
        /// <param name="attributeName">Name of attribute.</param>
        /// <returns>Extracted info.</returns>
        public IAttributeView ParseAttributeProperties(string attributeName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets info from parsed file about this element
        /// </summary>
        /// <returns>Extracted info.</returns>
        public IElementView ParseElementView()
        {
            throw new NotImplementedException();
        }

        private class DocumentParser
        {
            private XmlDocument document;

            public DocumentParser(XmlDocument document)
            {
                this.document = document;
            }


            public IDictionary<string, string> GetProperties()
            {
                var properties = new Dictionary<string, string>();
                foreach (XmlNode propertyNode in document.SelectNodes("/element/properties/property"))
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
                foreach (XmlNode attributeNode in document.SelectNodes("/element/attributes/attribute"))
                {
                    string name = attributeNode.Attributes["Name"].Value;
                    names.Add(name);
                }
                return names;
            }

            public Dictionary<string, string> GetAttributeProperties(string attributeName)
            {
                XmlNode found = null;
                foreach (XmlNode attributeNode in document.SelectNodes("/element/attributes/attribute"))
                {
                    string name = attributeNode.Attributes["Name"].Value;
                    if (name == attributeName)
                    {
                        found = attributeNode;
                    }
                }
                foreach (var property in found.ChildNodes)
                {

                }
                return null;
            }
        }
    }
}

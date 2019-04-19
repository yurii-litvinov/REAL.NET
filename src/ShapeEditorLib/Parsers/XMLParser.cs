using ShapeEditorLib.Parsers.XML;
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
        private XMLDocumentParser documentParser;

        /// <summary>
        /// Loads file.
        /// </summary>
        /// <param name="link">Link of file to load.</param>
        /// <exception cref="System.Xml.XmlException">Load or parse error in the XML.</exception>
        /// <exception cref="ArgumentException">Incorrect path or permission</exception>
        public void LoadFile(string link)
        {
            var document = new XmlDocument();
            document.Load(link);
            documentParser = new XMLDocumentParser(document);
        }

        /// <summary>
        /// Gets info from parsed file about this element
        /// </summary>
        /// <returns>Extracted info.</returns>
        public IElementView ParseElementView()
        {
            var elementProperties = documentParser.GetProperties();
            var attributeNames = documentParser.GetAttributesNames();
            var attributeList = new List<IAttributeView>();
            foreach (var attributeName in attributeNames)
            {
                attributeList.Add(ParseAttributeProperties(attributeName));
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets info about attribute.
        /// </summary>
        /// <param name="attributeName">Name of attribute.</param>
        /// <returns>Extracted info.</returns>
        /// <exception cref="InvalidOperationException">Invalid attribute definition format</exception>
        public IAttributeView ParseAttributeProperties(string attributeName)
        {
            var properties = documentParser.GetAttributeProperties(attributeName);
            string exampleValue = properties.ContainsKey("ExampleValue") ? properties["ExampleValue"] : null;
            int? order = null;
            if (properties.ContainsKey("OrderNumber"))
            {
                if (!Int32.TryParse(properties["OrderNumber"], out int parsedOrder))
                {
                    throw new InvalidOperationException("OrderNumber is not int");
                }
                order = parsedOrder;
            }
            bool isVisible = false;
            if (properties.ContainsKey("IsVisible"))
            {
                if (!bool.TryParse(properties["IsVisible"], out bool parsed))
                {
                    throw new InvalidOperationException("IsVisible is not true or false");
                }
                isVisible = parsed;
            }
            (int, int)? position = null;
            if (properties.ContainsKey("Position"))
            {
                string[] coordinates = properties["Position"].Split(' ');
                if (coordinates.Length != 2)
                {
                    throw new InvalidOperationException("Position format is not right");
                }
                if (Int32.TryParse(coordinates[0], out int x) && Int32.TryParse(coordinates[1], out int y))
                {
                    position = (x, y);
                }
                else
                {
                    throw new InvalidOperationException("Position format is not right");
                }
            }
            return new Wrappers.AttributeView(attributeName, exampleValue, order, isVisible, position);
        }
    }
}

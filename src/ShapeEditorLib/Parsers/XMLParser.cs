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
        /// Gets info about attribute.
        /// </summary>
        /// <param name="attributeName">Name of attribute.</param>
        /// <returns>Extracted info.</returns>
        public IAttributeView ParseAttributeProperties(string attributeName)
        {
            var attributeStringProperties = documentParser.GetAttributeProperties(attributeName);
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

        
    }
}

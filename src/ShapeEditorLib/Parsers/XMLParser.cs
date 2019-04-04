using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeEditorLib.Parsers
{
    public class XMLParser : IViewFileParser
    {
        public XMLParser(string link)
        {

        }

        public IAttributeView ParseAttributeProperties(string attributeName)
        {
            throw new NotImplementedException();
        }

        public IElementView ParseElementView()
        {
            throw new NotImplementedException();
        }
    }
}

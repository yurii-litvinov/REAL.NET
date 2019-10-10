using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeEditorLib.Parsers.Wrappers
{
    internal class ElementView : IElementView
    {
        public ElementView(string elementName, IDictionary<string, string> properties, IList<IAttributeView> attributes)
        {
            this.ElementName = elementName;
            this.Properties = properties;
            this.Attributes = attributes;
        }

        public string ElementName { get; }

        public IDictionary<string, string> Properties { get; }

        public IList<IAttributeView> Attributes { get; }
    }
}

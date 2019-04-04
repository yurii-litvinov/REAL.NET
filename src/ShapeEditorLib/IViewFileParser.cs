using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeEditorLib
{
    public interface IViewFileParser
    {
        IElementView ParseElementView();

        IAttributeView ParseAttributeProperties(string attributeName);
    }
}

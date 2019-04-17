using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeEditorLib
{
    /// <summary>
    /// Interface which is responsible for parsing files and extracting data fro them.
    /// </summary>
    public interface IViewFileParser
    {
        /// <summary>
        /// Loads file.
        /// </summary>
        /// <param name="link">Link of file to load.</param>
        void LoadFile(string link);

        /// <summary>
        /// Gets info from parsed file about this element
        /// </summary>
        /// <returns>Extracted info.</returns>
        IElementView ParseElementView();

        /// <summary>
        /// Gets info about attribute.
        /// </summary>
        /// <param name="attributeName">Name of attribute.</param>
        /// <returns>Extracted info.</returns>
        IAttributeView ParseAttributeProperties(string attributeName);
    }
}

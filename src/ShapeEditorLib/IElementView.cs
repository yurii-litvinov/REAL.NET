using System.Collections.Generic;

namespace ShapeEditorLib
{
    /// <summary>
    /// Represents info about how element should be shown.
    /// </summary>
    public interface IElementView
    {
        /// <summary>
        /// Name of element.
        /// </summary>
        string ElementName { get; }

        /// <summary>
        /// Properties of element.
        /// </summary>
        IDictionary<string, string> Properties { get; }

        /// <summary>
        /// List of attributes.
        /// </summary>
        IList<IAttributeView> Attributes { get; }
    }
}

using Repo;

namespace ShapeEditorLib
{
    /// <summary>
    /// Represents info how attributes should be represented.
    /// </summary>
    public interface IAttributeView
    {
        /// <summary>
        /// Name of attribute.
        /// </summary>
        string AttributeName { get; }

        /// <summary>
        /// Value to show as an example of some value of attribute.
        /// </summary>
        string ExampleValue { get; set; }

        /// <summary>
        /// Number of attribute in list
        /// </summary>
        int? OrderNumber { get; set; }

        /// <summary>
        /// Is this attribute shown on screen. 
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Position of attribute corresponding to element. 
        /// </summary>
        (int, int)? Position { get; set; }
    }
}

/*using GraphX.PCL.Common.Models;

namespace EditorPrototype
{
    public class DataVertex : VertexBase
    {
        public string Text { get; set; }
        public string Name { get; set; }
        public int ImageId { get; set; }

        public bool IsBlue { get; set; }

        #region Calculated or static props

        public override string ToString()
        {
            return Text;
        }

        #endregion

        /// <summary>
        /// Default constructor for this class
        /// (required for serialization).
        /// </summary>
        public DataVertex() : this(string.Empty)
        {
        }

        public DataVertex(string text = "")
        {
            Text = string.IsNullOrEmpty(text) ? "New Vertex" : text;
        }
    }
}*/


using GraphX;
using GraphX.PCL.Common.Models;

namespace EditorPrototype
{
    /* DataVertex is the data class for the vertices. It contains all custom vertex data specified by the user.
     * This class also must be derived from VertexBase that provides properties and methods mandatory for
     * correct GraphX operations.
     * Some of the useful VertexBase members are:
     *  - ID property that stores unique positive identfication number. Property must be filled by user.
     *  
     */

    public class DataVertex : VertexBase
    {
        /// <summary>
        /// Some string property for example purposes
        /// </summary>
        public string Name { get; set; }
        public string Key { get; set; }

        #region Calculated or static props

        public override string ToString()
        {
            return Name;
        }


        #endregion

        /// <summary>
        /// Default parameterless constructor for this class
        /// (required for YAXLib serialization)
        /// </summary>
        public DataVertex() : this("")
        {
        }

        public DataVertex(string text = "")
        {
            Name = text;
        }
    }
}
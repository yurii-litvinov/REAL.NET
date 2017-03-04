using GraphX.PCL.Common.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace EditorPrototype
{
    /* DataVertex is the data class for the vertices. It contains all custom vertex data specified by the user.
     * This class also must be derived from VertexBase that provides properties and methods mandatory for
     * correct GraphX operations.
     * Some of the useful VertexBase members are:
     *  - ID property that stores unique positive identfication number. Property must be filled by user.
     *  
     */

    public class DataVertex : VertexBase, INotifyPropertyChanged
    {
        public enum VertexTypeEnum
        {
            Node,
            Attribute
        }

        public class Attribute
        {
            public string Name { get; set; }

            public string Type { get; set; }

            public string Value { get; set; }
        }

        /// <summary>
        /// Some string property for example purposes
        /// </summary>
        public string Name { get; set; }

        public string Key { get; set; }

        private Brush color = Brushes.Green;
        private VertexTypeEnum vertexType = VertexTypeEnum.Node;
        private IList<Attribute> attributes = new List<Attribute>();

        public Brush Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                OnPropertyChanged(nameof(Color));
            }
        }

        public VertexTypeEnum VertexType
        {
            get
            {
                return this.vertexType;
            }
            set
            {
                this.vertexType = value;
                OnPropertyChanged(nameof(this.VertexType));
            }
        }

        public IList<Attribute> Attributes
        {
            get
            {
                return attributes;
            }
            set
            {
                attributes = value;
                OnPropertyChanged(nameof(Attributes));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
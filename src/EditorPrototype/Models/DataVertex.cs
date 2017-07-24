namespace EditorPrototype
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Media;
    using GraphX.PCL.Common.Models;

    /* DataVertex is the data class for the vertices. It contains all custom vertex data specified by the user.
     * This class also must be derived from VertexBase that provides properties and methods mandatory for
     * correct GraphX operations.
     * Some of the useful VertexBase members are:
     *  - ID property that stores unique positive identfication number. Property must be filled by user.
     */

    public class DataVertex : VertexBase, INotifyPropertyChanged
    {
        private Brush color = Brushes.Green;
        private VertexTypeEnum vertexType = VertexTypeEnum.Node;
        private IList<Attribute> attributes = new List<Attribute>();
        private string picture = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataVertex"/> class.
        /// Default parameterless constructor for this class (required for YAXLib serialization)
        /// </summary>
        public DataVertex()
            : this(string.Empty)
        {
        }

        public DataVertex(string text = "")
        {
            this.Name = text;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public enum VertexTypeEnum
        {
            Node,
            Attribute,
        }

        /// <summary>
        /// Gets or sets some string property for example purposes
        /// </summary>
        public string Name { get; set; }

        public Repo.INode Node { get; set; }

        public Brush Color
        {
            get
            {
                return this.color;
            }

            set
            {
                this.color = value;
                this.OnPropertyChanged(nameof(this.Color));
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
                this.OnPropertyChanged(nameof(this.VertexType));
            }
        }

        public IList<Attribute> Attributes
        {
            get
            {
                return this.attributes;
            }

            set
            {
                this.attributes = value;
                this.OnPropertyChanged(nameof(this.Attributes));
            }
        }

        public string Picture
        {
            get
            {
                return "pack://application:,,,/" + this.picture;
            }

            set
            {
                this.picture = value;
                this.OnPropertyChanged(nameof(this.Picture));
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        public class Attribute
        {
            public string Name { get; set; }

            public string Type { get; set; }

            public string Value { get; set; }
        }
    }
}

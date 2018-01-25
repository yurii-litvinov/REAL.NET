using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using GraphX.PCL.Common.Models;
using Repo;

namespace WpfEditor.ViewModel
{
    /// <summary>
    /// NodeViewModel is the data class for the vertices. It contains all custom vertex data specified by the user.
    /// This class also must be derived from VertexBase that provides properties and methods mandatory for
    /// correct GraphX operations.
    /// </summary>
    public class NodeViewModel : VertexBase, INotifyPropertyChanged
    {
        private Brush color = Brushes.Green;
        private VertexTypeEnum vertexType = VertexTypeEnum.Node;
        private IList<Attribute> attributes = new List<Attribute>();
        private string picture = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeViewModel"/> class.
        /// Default parameterless constructor for this class (required for YAXLib serialization)
        /// </summary>
        public NodeViewModel()
            : this(string.Empty)
        {
        }

        public NodeViewModel(string text = "")
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

        public INode Node { get; set; }

        public Brush Color
        {
            get => this.color;

            set
            {
                this.color = value;
                this.OnPropertyChanged(nameof(this.Color));
            }
        }

        public VertexTypeEnum VertexType
        {
            get => this.vertexType;

            set
            {
                this.vertexType = value;
                this.OnPropertyChanged(nameof(this.VertexType));
            }
        }

        public IList<Attribute> Attributes
        {
            get => this.attributes;

            set
            {
                this.attributes = value;
                this.OnPropertyChanged(nameof(this.Attributes));
            }
        }

        public string Picture
        {
            get => "pack://application:,,,/" + this.picture;

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
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public class Attribute
        {
            private string value;
            private readonly IAttribute attribute;

            public Attribute(IAttribute attribute, string name, string type)
            {
                this.attribute = attribute;
                this.Name = name;
                this.Type = type;
            }

            public string Name { get; }

            public string Type { get; }

            public string Value
            {
                get => this.value;

                set
                {
                    this.attribute.StringValue = value;
                    this.value = value;
                }
            }
        }
    }
}


using System.Collections.Generic;
using System.ComponentModel;
using GraphX.Measure;
using GraphX.PCL.Common.Models;
using Repo.DataLayer;

namespace WpfEditor.ViewModel
{
    public class EdgeViewModel : EdgeBase<NodeViewModel>, INotifyPropertyChanged
    {
        private EdgeTypeEnum edgeType = EdgeTypeEnum.Association;
        private IList<Attribute> attributes = new List<Attribute>();

        public EdgeViewModel(NodeViewModel source, NodeViewModel target, double weight = 1)
            : base(source, target, weight)
        {
            this.Angle = 90;
        }

        public EdgeViewModel()
            : base(null, null, 1)
        {
            this.Angle = 90;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public enum EdgeTypeEnum
        {
            Generalization,
            Association,
            Type,
            Attribute,
        }

        public bool ArrowTarget { get; set; }

        public double Angle { get; set; }

        public override Point[] RoutingPoints { get; set; }

        public string Text { get; set; }

        public IList<Attribute> Attributes
        {
            get => this.attributes;
            set
            {
                this.attributes = value;
                this.OnPropertyChanged(nameof(this.Attributes));
            }
        }
        public string ToolTipText { get; set; }

        public EdgeTypeEnum EdgeType
        {
            get => this.edgeType;

            set
            {
                this.edgeType = value;
                this.OnPropertyChanged(nameof(this.EdgeType));
            }
        }

        public void OnPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public override string ToString() => this.Text;

        public class Attribute
        {
            public string Name { get; set; }

            public string Type { get; set; }

            public string Value { get; set; }
        }
    }
}

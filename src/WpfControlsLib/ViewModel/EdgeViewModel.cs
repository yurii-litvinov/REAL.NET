using System.Collections.Generic;
using System.ComponentModel;
using GraphX.Measure;
using GraphX.PCL.Common.Interfaces;
using GraphX.PCL.Common.Models;
using Repo;

namespace WpfControlsLib.ViewModel
{
    public class EdgeViewModel : EdgeBase<NodeViewModel>, INotifyPropertyChanged, IGraphXEdge<NodeViewModel>
    {
        private EdgeTypeEnum edgeType = EdgeTypeEnum.Association;
        private IList<AttributeViewModel> attributes = new List<AttributeViewModel>();

        private string text;

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

        public int IndexOfInflectionPoint { get; set; }

        public IEdge Edge { get; set; }

        public string Text
        {
            get => this.text;

            set
            {
                this.text = value;
                this.OnPropertyChanged(nameof(this.Text));
            }
        }
        
        public IList<AttributeViewModel> Attributes
        {
            get => this.attributes;

            set
            {
                this.attributes = value;
                this.OnPropertyChanged();
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

        public void OnPropertyChanged(string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        
        public override string ToString() => this.Text;
    }
}

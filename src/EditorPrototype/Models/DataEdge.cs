using System.Collections.Generic;

namespace EditorPrototype
{
    using System;
    using System.ComponentModel;
    using GraphX.Measure;
    using GraphX.PCL.Common.Models;

    [Serializable]
    public class DataEdge : EdgeBase<DataVertex>, INotifyPropertyChanged
    {
        private EdgeTypeEnum edgeType = EdgeTypeEnum.Association;
        private IList<Attribute> attributes = new List<Attribute>();

        public DataEdge(DataVertex source, DataVertex target, double weight = 1)
            : base(source, target, weight)
        {
            this.Angle = 90;
        }

        public DataEdge()
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
            get
            {
                return this.edgeType;
            }

            set
            {
                this.edgeType = value;
                this.OnPropertyChanged(nameof(this.EdgeType));
            }
        }

        public void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        public override string ToString()
        {
            return this.Text;
        }

        public class Attribute
        {
            public string Name { get; set; }

            public string Type { get; set; }

            public string Value { get; set; }
        }
}
}

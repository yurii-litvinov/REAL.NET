namespace EditorPrototype
{
    using System;
    using System.ComponentModel;
    using FileSerialization;
    using GraphX.Measure;
    using GraphX.PCL.Common.Models;
    using YAXLib;

    [Serializable]
    public class DataEdge : EdgeBase<DataVertex>, INotifyPropertyChanged
    {
        private EdgeTypeEnum edgeType = EdgeTypeEnum.Association;

        private string text;

        public bool ArrowTarget { get; set; }

        public double Angle { get; set; }

        [YAXCustomSerializer(typeof(YAXPointArraySerializer))]
        public override Point[] RoutingPoints { get; set; }

        public enum EdgeTypeEnum
        {
            Generalization,
            Association,
            Type,
            Attribute
        }

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
                
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
                OnPropertyChanged(nameof(this.Text));
            }
        }

        public string ToolTipText { get; set; }

        public override string ToString()
        {
            return this.Text;
        }

        public EdgeTypeEnum EdgeType
        {
            get
            {
                return this.edgeType;
            }

            set
            {
                this.edgeType = value;
                OnPropertyChanged(nameof(EdgeType));
            }
        }
        
        public void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
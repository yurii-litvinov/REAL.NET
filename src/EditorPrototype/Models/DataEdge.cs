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
        public enum EdgeTypeEnum
        {
            Generalization,
            Association,
            Type,
            Attribute
        }

        [YAXCustomSerializer(typeof(YAXPointArraySerializer))]
        public override Point[] RoutingPoints { get; set; }

        public DataEdge(DataVertex source, DataVertex target, double weight = 1)
            : base(source, target, weight)
        {
            Angle = 90;
        }

        public DataEdge()
            : base(null, null, 1)
        {
            Angle = 90;
        }

        public bool ArrowTarget { get; set; }

        public double Angle { get; set; }

        /// <summary>
        /// Node main description (header)
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public string ToolTipText { get; set; }

        public override string ToString()
        {
            return Text;
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private EdgeTypeEnum edgeType = EdgeTypeEnum.Association;

        private string text;
    }
}
namespace EditorPrototype
{
    using System;
    using System.ComponentModel;
    using GraphX.Measure;
    using GraphX.PCL.Common.Models;
    using FileSerialization;
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
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }

        }

        public string ToolTipText { get; set; }

        public override string ToString()
        {
            return Text;
        }

        private EdgeTypeEnum edgeType = EdgeTypeEnum.Association;

        public EdgeTypeEnum EdgeType
        {
            get
            {
                return edgeType;
            }

            set
            {
                edgeType = value;
                OnPropertyChanged(nameof(EdgeType));
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

}
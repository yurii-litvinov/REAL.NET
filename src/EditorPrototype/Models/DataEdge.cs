namespace EditorPrototype
{
    using System;
    using System.ComponentModel;
    using EditorPrototype.FileSerialization;
    using GraphX.Measure;
    using GraphX.PCL.Common.Models;
    using YAXLib;

    [Serializable]
    public class DataEdge : EdgeBase<DataVertex>, INotifyPropertyChanged
    {
        /// <summary>
        /// Node main description (header)
        /// </summary>
        private string text;

        private EdgeTypeEnum edgeType = EdgeTypeEnum.Association;

        public DataEdge(DataVertex source, DataVertex target, bool isViolation, double weight = 1)
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
            Attribute
        }

        [YAXCustomSerializer(typeof(YAXPointArraySerializer))]
        public override Point[] RoutingPoints { get; set; }

        public bool ArrowTarget { get; set; }

        public double Angle { get; set; }

        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
                this.OnPropertyChanged(nameof(this.Text));
            }
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
                this.OnPropertyChanged(nameof(this.EdgeType));
            }
        }

        public string ToolTipText { get; set; }

        public override string ToString()
        {
            return this.Text;
        }

        public void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
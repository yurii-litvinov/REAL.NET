using System.ComponentModel;
using GraphX;
using System;
using GraphX.Measure;
using GraphX.PCL.Common.Models;
using EditorPrototype.FileSerialization;
using YAXLib;
using GraphX.Controls;
using System.Windows.Media;

namespace EditorPrototype
{
    [Serializable]
    public class DataEdge : EdgeBase<DataVertex>, INotifyPropertyChanged
    {
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
        public string Text { get { return _text; } set { _text = value; OnPropertyChanged(nameof(Text)); } }
        public string ToolTipText { get; set; }

        public override string ToString()
        {
            return Text;
        }

        private EdgeDashStyle dashStyle = EdgeDashStyle.Solid;

        public EdgeDashStyle DashStyle
        {
            get
            {
                return dashStyle;
            }
            set
            {
                dashStyle = value;
                OnPropertyChanged(nameof(DashStyle));
            }
        }

        private Brush pointerFillColor = new SolidColorBrush(Colors.White);

        public Brush PointerFillColor
        {
            get
            {
                return pointerFillColor;
            }
            set
            {
                pointerFillColor = value;
                OnPropertyChanged(nameof(PointerFillColor));
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
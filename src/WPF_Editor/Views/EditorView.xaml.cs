using System.Windows;
using GraphX.PCL.Common.Models;
using WPF_Editor.ViewModels;

namespace WPF_Editor.Views
{
    public class DataVertex : VertexBase
    {
        /// <summary>
        ///     Some string property for example purposes
        /// </summary>
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    public class DataEdge : EdgeBase<DataVertex>
    {
        public DataEdge(DataVertex source, DataVertex target, double weight = 1)
            : base(source, target, weight)
        {
        }

        public DataEdge()
            : base(null, null, 1)
        {
        }

        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class EditorView : Window
    {
        public EditorView()
        {
            //EditorViewModel's initialization has to be done before InitializeComponent();
            var x = new EditorViewModel();
            InitializeComponent();
            DataContext = x;
        }
    }
}
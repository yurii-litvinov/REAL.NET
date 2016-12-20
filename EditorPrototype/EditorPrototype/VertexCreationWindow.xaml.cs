using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using GraphX.Controls;

namespace EditorPrototype
{
    /// <summary>
    /// Логика взаимодействия для VertexCreationWindow.xaml
    /// </summary>
    public partial class VertexCreationWindow : Window
    {
        public VertexCreationWindow()
        {
            InitializeComponent();
            Closing += CancelClosing;
            vertexNameBox.TextChanged += CheckControl;
        }

        private void CheckControl(object sender, TextChangedEventArgs e)
        {
            if (vertexNameBox.Text.Length > 0)
            {
                addVertexBut.IsEnabled = true;
            }
            else
            {
                addVertexBut.IsEnabled = false;
            }
        }

        private void CancelClosing(object sender, CancelEventArgs e)
        {
            if (!MainWindow.close)
            {
                e.Cancel = true;
                ClearWindow(sender, e);
            }
        }

        public event EventHandler VertexAddButClicked;
        
        private void ClearWindow(object sender, EventArgs e)
        {
            vertexNameBox.Text = string.Empty;
            vertexKeyBox.Text = string.Empty;
            addVertexBut.IsEnabled = false;
            Visibility = Visibility.Hidden;
        }

        private void addVertexBut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.newVertex = new DataVertex(vertexNameBox.Text);
            MainWindow.newVertex.Key = vertexKeyBox.Text;
            MainWindow.dataGraph.AddVertex(MainWindow.newVertex);
            /*var vc1 = new VertexControl(dataVertex);
            g_Area.AddVertex(dataVertex, vc1);*/
            ClearWindow(sender, e);
            VertexAddButClicked(this, EventArgs.Empty);
        }

        private void vertexCancelBut_Click(object sender, RoutedEventArgs e)
        {
            ClearWindow(sender, e);
        }
    }
}
